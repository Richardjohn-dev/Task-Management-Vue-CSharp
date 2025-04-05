﻿using Domain.Tasks.Synchronization;
using Hangfire;
using HangfireParallelTasks.Features.Tasks.Constants;
using System.Collections.Concurrent;

namespace Domain.Tasks.Enqueue;
public class DomainTaskQueue
{
    public event EventHandler? BackgroundServiceTasksCompleted;

    public event EventHandler<TaskEventArgs>? TaskStatusChanged;
    public event EventHandler<TaskFailedEventArgs>? TaskFailed;
    public event EventHandler<DomainTaskResultUI>? TaskCompleted;

    private bool _backgroundServiceProcessing = false;

    private readonly SemaphoreSlim _taskQueueAccess = new SemaphoreSlim(1, 1);


    private readonly ConcurrentDictionary<TaskKey, byte> _backgroundJobIds = new();

    public DomainTaskQueue(string[] QueueNames)
    {
        _domainTaskQueues = new ConcurrentDictionary<DomainTaskQueueName, QueueStatus>(
            QueueNames.Select(QueueName => new KeyValuePair<DomainTaskQueueName, QueueStatus>(new DomainTaskQueueName(QueueName), QueueStatus.Empty))
        );
    }



    //  Monitor queue / if a queue is CurrentlyProcessingThisgroupId or empty
    private readonly ConcurrentDictionary<DomainTaskQueueName, QueueStatus> _domainTaskQueues = new();


    // groupId / queue name
    // -- tracking what Group currently processing and in which queue
    private readonly ConcurrentDictionary<SharedGroupIdentifier, (DomainTaskQueueName, GroupItem)> _currentlyProcessingGroups = new();

    private readonly ConcurrentDictionary<SharedGroupIdentifier, Queue<DomainTaskInfo>> _currentlyProcessingGroupOverflow = new();


    private readonly ConcurrentDictionary<SharedGroupIdentifier, Queue<DomainTaskInfo>> _backgroundServiceOverflow = new();


    private readonly ConcurrentDictionary<SharedGroupIdentifier, Queue<DomainTaskInfo>> _allQueuesBusyOverflow = new();







    internal async Task<Result<TaskStatusUpdateResponse>> TryEnqueueDomainTask(DomainTaskInfo task, bool NotifyEnqueued = true)
    {
        if (!await _taskQueueAccess.WaitAsync(TimeSpan.FromSeconds(10)))
            return Result.Error("Failed getting a ticket... must be busy.");
        try
        {
            return EnqueueOrCacheTask(task);
        }
        finally
        {
            _taskQueueAccess.Release();
        }

    }

    internal async Task<Result> TryEnqueueDomainTasks(DomainTaskInfo[] tasks)
    {
        if (!await _taskQueueAccess.WaitAsync(TimeSpan.FromSeconds(10)))
            return Result.Error("Failed getting a ticket... must be busy.");
        try
        {
            foreach (var task in tasks)
            {
                _ = EnqueueOrCacheTask(task);
            }
            return Result.Success();
        }
        finally
        {
            _taskQueueAccess.Release();
        }
    }

    private TaskStatusUpdateResponse EnqueueOrCacheTask(DomainTaskInfo task, bool NotifyEnqueued = true)
    {
        try
        {
            if (CurrentlyProcessingGroup(task.Details.Group))
            {
                return AddToCurrentlyProcessingOverflow(task);
            }

            var getNextAvailableQueue = GetNextAvailableQueue();

            if (!getNextAvailableQueue.IsSuccess)
            {
                return AddToQueuesFullOverflow(task);
            }

            return TryEnqueueNewSynchronizationTask(getNextAvailableQueue.Value, task);
        }
        finally
        {
            if (NotifyEnqueued)
                NotifySynchronizationMappingTaskEnqueued(task);
        }
    }


    private TaskStatusUpdateResponse AddToCurrentlyProcessingOverflow(DomainTaskInfo task)
    {
        var groupId = task.Details.Group;

        _currentlyProcessingGroupOverflow.AddOrUpdate(groupId,
            id => new Queue<DomainTaskInfo>(new[] { task }), // create new 
           (id, queue) => // or enqueue existing
           {
               queue.Enqueue(task);
               return queue;
           });


        return TaskHelper.Enqueued(task);
    }

    private TaskStatusUpdateResponse AddToQueuesFullOverflow(DomainTaskInfo task)
    {
        var groupId = task.Details.Group;

        _allQueuesBusyOverflow.AddOrUpdate(task.Details.Group,
            id => new Queue<DomainTaskInfo>(new[] { task }), // create new 
           (id, queue) => // or enqueue existing
           {
               queue.Enqueue(task);
               return queue;
           });

        return TaskHelper.Enqueued(task);

    }

    private Result<TaskStatusUpdateResponse> TryEnqueueNewSynchronizationTask(DomainTaskQueueName queue, DomainTaskInfo task)
    {
        try
        {
            EnqueueSynchronizationTask(queue, task);

            _domainTaskQueues[queue] = QueueStatus.NotAvailable;
            _currentlyProcessingGroups.TryAdd(task.Details.Group, (queue, task.Details.Item));

            LogEnqueued(queue, task);
            NotifySynchronizationMappingTaskEnqueued(task);

            return TaskHelper.Enqueued(task);
        }
        catch (Exception e)
        {
            return Result.Error(e.Message);
        }

    }
    private void EnqueueSynchronizationTask(DomainTaskQueueName queue, DomainTaskInfo task)
        => BackgroundJob.Enqueue<DomainTaskProcessor>(x => x.TryProcessSynchronizationTask(queue.Name, queue, task));

    private void LogEnqueued(DomainTaskQueueName queueName, DomainTaskInfo taskInfo)
    {
        Console.WriteLine();
        Console.WriteLine($"--> ENQUEUED #{queueName.Number} ({taskInfo.Details.Group.Id})");
        Console.WriteLine();
    }

    internal async Task OnTaskComplete(DomainTaskQueueName queue, DomainTaskInfo taskInfo)
    {
        await _taskQueueAccess.WaitAsync();
        try
        {

            if (taskInfo.Source == TaskTriggeredBy.BackgroundService)
            {
                if (_backgroundJobIds.ContainsKey(taskInfo.Details.TaskKey))
                    _backgroundJobIds.TryRemove(taskInfo.Details.TaskKey, out _);
            }

            var groupId = taskInfo.Details.Group;

            var getNextTaskResult = TryGetNextTaskForGroup(groupId);

            if (getNextTaskResult.IsSuccess)
            {
                TryEnqueueNewSynchronizationTask(queue, getNextTaskResult.Value);
            }
            else
            {
                // cleanup currently processing Group
                _currentlyProcessingGroups.TryRemove(groupId, out _);
                _currentlyProcessingGroupOverflow.TryRemove(groupId, out _);
                _domainTaskQueues[queue] = QueueStatus.Empty;

                OnQueueAvailable(queue);
            }
        }
        finally
        {
            _taskQueueAccess.Release();
        }
    }

    private void LogQueueAvailable(DomainTaskQueueName queueName)
    {
        Console.WriteLine();
        Console.WriteLine($">>>>>> QUEUE #{queueName.Number} AVAILABLE<<<<<<<<");
        Console.WriteLine();
    }

    private void OnQueueAvailable(DomainTaskQueueName QueueName)
    {
        LogQueueAvailable(QueueName);

        // prioritize overflow from UI first.

        if (_allQueuesBusyOverflow.Any())
        {
            var startNextGroupQueue = _allQueuesBusyOverflow.First();
            var groupId = startNextGroupQueue.Key;
            var GroupQueue = startNextGroupQueue.Value;
            if (GroupQueue.Count > 0)
            {
                var nextTask = GroupQueue.Dequeue();
                TryEnqueueNewSynchronizationTask(QueueName, nextTask);

                // add remaining tasks to Group overflow
                if (GroupQueue.Count > 0)
                    _currentlyProcessingGroupOverflow.AddOrUpdate(groupId, GroupQueue, (id, queue) => queue);


            }
            _allQueuesBusyOverflow.TryRemove(groupId, out _);

        }



        // load background service overflow next.
        if (_backgroundServiceOverflow.Any())
        {
            var startNextGroupQueue = _backgroundServiceOverflow.First();
            var groupId = startNextGroupQueue.Key;
            var GroupTaskQueue = startNextGroupQueue.Value;
            if (GroupTaskQueue.Count > 0)
            {
                var nextTask = GroupTaskQueue.Dequeue();
                TryEnqueueNewSynchronizationTask(QueueName, nextTask);

                // add remaining tasks to Group overflow
                if (GroupTaskQueue.Count > 0)
                    _currentlyProcessingGroupOverflow.AddOrUpdate(groupId, GroupTaskQueue, (id, queue) => queue);


            }
            _backgroundServiceOverflow.TryRemove(groupId, out _);

            if (_backgroundServiceOverflow.Count == 0)
            {
                BackgroundServiceTasksCompleted?.Invoke(this, EventArgs.Empty);
            }
            return;
        }

        if (_backgroundServiceProcessing && _backgroundJobIds.Count == 0)
        {
            BackgroundServiceTasksCompleted?.Invoke(this, EventArgs.Empty);
            _backgroundServiceProcessing = false;
        }
    }


    private bool CurrentlyProcessingGroup(SharedGroupIdentifier group)
    {
        return _currentlyProcessingGroups.ContainsKey(group);
    }
    private Result<DomainTaskInfo> TryGetNextTaskForGroup(SharedGroupIdentifier groupId)
    {
        if (!_currentlyProcessingGroupOverflow.ContainsKey(groupId))
            return Result.Error();

        if (_currentlyProcessingGroupOverflow[groupId].Count() == 0)
            return Result.Error();

        var nextTask = _currentlyProcessingGroupOverflow[groupId].Dequeue();

        return nextTask is null ? Result.Error() : Result.Success(nextTask);
    }



    private Result<DomainTaskQueueName> GetNextAvailableQueue()
    {
        // just return queue name
        var availbleQueue = _domainTaskQueues.Where(x => x.Value == QueueStatus.Empty)
         .Select(x => x.Key)
         .FirstOrDefault();

        return availbleQueue is null ? Result.Error("Service busy.. no available queues right now.") : availbleQueue;

    }



    // Background service enqueue tasks entry point

    internal async Task<Result> TryEnqueueTasksFromBackgroundService(DomainTaskInfo[] allTasks)
    {
        if (!await _taskQueueAccess.WaitAsync(TimeSpan.FromSeconds(10)))
            return Result.Error("Failed getting a ticket... must be busy.");
        try
        {

            var taskGroups = allTasks.GroupBy(x => x.Details.Group);

            foreach (var taskGroup in taskGroups)
            {
                var groupId = taskGroup.Key;
                var taskQueue = new Queue<DomainTaskInfo>(taskGroup.ToArray());

                if (CurrentlyProcessingGroup(taskGroup.Key))
                {
                    var enqueued = AddRemainingTasksToCurrentlyProcessingOverflow(groupId, taskQueue);
                }
                else
                {

                    var getQueueResult = GetNextAvailableQueue();
                    if (getQueueResult.IsSuccess)
                    {
                        var firstTask = taskQueue.Dequeue();
                        _ = TryEnqueueNewSynchronizationTask(getQueueResult.Value, firstTask);
                        _ = AddRemainingTasksToCurrentlyProcessingOverflow(groupId, taskQueue);

                    }
                    else
                    {
                        // no queue, add to overflows
                        AddToQueuesFullOverflow(groupId, taskQueue, TaskTriggeredBy.BackgroundService);
                    }
                }


                AddBackgroundJobIds(taskGroup.ToArray());
            }
            return Result.Success();
        }
        finally
        {
            _taskQueueAccess.Release();
        }
    }

    private void AddToQueuesFullOverflow(SharedGroupIdentifier groupId, Queue<DomainTaskInfo> taskQueue, TaskTriggeredBy backgroundService)
    {
        if (taskQueue.Count == 0)
            return;

        _allQueuesBusyOverflow.AddOrUpdate(groupId,
              _ => new Queue<DomainTaskInfo>(taskQueue),
              (_, existingQueue) =>
              {

                  foreach (var task in taskQueue)
                  {
                      existingQueue.Enqueue(task);
                  }
                  return existingQueue;
              });
        return;
    }

    private TaskStatusUpdateResponse[] AddRemainingTasksToCurrentlyProcessingOverflow(SharedGroupIdentifier groupId, Queue<DomainTaskInfo> taskQueue)
    {
        if (taskQueue.Count == 0)
            return Array.Empty<TaskStatusUpdateResponse>();

        _currentlyProcessingGroupOverflow.AddOrUpdate(groupId,
            _ => new Queue<DomainTaskInfo>(taskQueue),
            (_, existingQueue) =>
            {
                foreach (var task in taskQueue)
                {
                    existingQueue.Enqueue(task);
                }
                return existingQueue;
            });

        return taskQueue.Select(task => TaskHelper.Enqueued(task)).ToArray();

    }



    private void AddBackgroundJobIds(DomainTaskInfo[] tasksByGroupId)
    {
        var jobIds = tasksByGroupId
           .Select(x => x.Details.TaskKey);

        foreach (var job in jobIds)
        {
            _backgroundJobIds.TryAdd(job, 0);
        }
    }






    // Handle Task Updates as fits ..
    public void NotifySynchronizationTaskFailed(DomainTaskInfo taskInfo, params string[] errors)
         => Console.WriteLine($"Task Error : {errors}");
    //=> TaskFailed?.Invoke(this, new (taskInfo, errors));

    internal void SendSynchonizationResults(DomainTaskResultUI synchronizationResult)
          => Console.WriteLine($"Task Complete : {synchronizationResult}");
    //=> TaskCompleted?.Invoke(this, synchronizationResult);

    public void NotifySynchronizationMappingTaskEnqueued(DomainTaskInfo taskInfo)
        => TaskStatusUpdate(DomainTaskStatus.Enqueued, taskInfo);

    public void NotifySynchronizationTaskStarted(DomainTaskInfo taskInfo)
        => TaskStatusUpdate(DomainTaskStatus.Processing, taskInfo);

    private void TaskStatusUpdate(DomainTaskStatus status, DomainTaskInfo taskInfo)
        => Console.WriteLine($"{status} : {taskInfo}");
    // Notify frontend via SignalR here..
    //=> TaskStatusChanged?.Invoke(this, new TaskEventArgs(taskInfo, status));



    internal async Task<bool> GroupIsCurrentlyProcessing(SharedGroupIdentifier groupId)
    {
        if (!await _taskQueueAccess.WaitAsync(TimeSpan.FromSeconds(10)))
            return false;

        try
        {
            return _currentlyProcessingGroups.ContainsKey(groupId);
        }
        finally
        {
            _taskQueueAccess.Release();
        }
    }

    // Add endpoint to clear tasks.
    internal async Task ClearInternalTasks()
    {
        if (!await _taskQueueAccess.WaitAsync(TimeSpan.FromSeconds(10)))
            return;

        try
        {
            _currentlyProcessingGroups.Clear();
            _currentlyProcessingGroupOverflow.Clear();
            _backgroundJobIds.Clear();
            _backgroundServiceOverflow.Clear();
            _allQueuesBusyOverflow.Clear();
            _domainTaskQueues.Clear();
            BackgroundServiceTasksCompleted?.Invoke(this, EventArgs.Empty);
        }
        finally
        {
            _taskQueueAccess.Release();
        }

    }


}

