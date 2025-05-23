﻿using Domain.Tasks.Enqueue;
using Microsoft.AspNetCore.SignalR;

namespace TaskManager.SignalR;

public class TaskHub : Hub
{
    public static readonly HashSet<string> ActiveConnections = new();
    private readonly DomainTaskQueue _taskQueue;

    public TaskHub(DomainTaskQueue domainTaskQueue)
    {
        _taskQueue = domainTaskQueue;
    }

    public override Task OnConnectedAsync()
    {
        ActiveConnections.Add(Context.ConnectionId);
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        ActiveConnections.Remove(Context.ConnectionId);
        return base.OnDisconnectedAsync(exception);
    }

    public async Task GetLatestTaskStatuses(List<string> taskKeys)
    {
        var taskStatuses = await _taskQueue.GetTaskStatusesAsync(taskKeys);
        await Clients.Caller.SendAsync(SignalRTaskApi.TaskStatusUpdates, taskStatuses);
    }

    public static bool IsClientConnected(string connectionId) => ActiveConnections.Contains(connectionId);
}

