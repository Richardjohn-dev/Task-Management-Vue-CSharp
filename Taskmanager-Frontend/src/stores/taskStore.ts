// stores/taskStore.ts
import { defineStore } from 'pinia'
import { api } from '@/services/api'
import type { TaskStatus, TaskStatusUpdateResponse, DomainEntityDetails } from '@/models/types'

interface TasksState {
  taskStatuses: Record<string, TaskStatus>
  enqueuingTask: boolean
  enqueueError: string | null
}

export const useTaskStore = defineStore('tasks', {
  state: (): TasksState => ({
    taskStatuses: {},
    enqueuingTask: false,
    enqueueError: null,
  }),

  getters: {
    getTaskStatus: (state) => (taskKey: string) => {
      return state.taskStatuses[taskKey] || null
    },

    hasTask: (state) => (taskKey: string) => {
      return taskKey in state.taskStatuses
    },
  },

  actions: {
    updateTaskStatus(response: TaskStatusUpdateResponse) {
      console.log('Updating task status:', response)
      this.taskStatuses[response.taskKey.value] = response.status
    },

    updateTaskStatuses(responses: TaskStatusUpdateResponse[]) {
      responses.forEach((response) => {
        this.taskStatuses[response.taskKey.value] = response.status
      })
    },

    clearTaskStatus(taskKey: string) {
      if (taskKey in this.taskStatuses) {
        delete this.taskStatuses[taskKey]
      }
    },

    async enqueueDomainTask(details: DomainEntityDetails) {
      this.enqueuingTask = true
      this.enqueueError = null

      try {
        const response = await api.enqueueDomainTask(details)
        this.updateTaskStatus(response)
        return true
      } catch (err) {
        this.enqueueError = err instanceof Error ? err.message : 'An unknown error occurred'
        return false
      } finally {
        this.enqueuingTask = false
      }
    },
  },
})
