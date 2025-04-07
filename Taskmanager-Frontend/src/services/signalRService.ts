// services/signalRService.ts
import * as signalR from '@microsoft/signalr'
import { useTaskStore } from '@/stores/taskStore'
import { useSignalRStore } from '@/stores/signalRStore'
import { getBaseUrl } from '@/utils/baseUrl'
import type { TaskStatusUpdateResponse } from '@/models/types'

class SignalRService {
  private readonly baseUrl: string = getBaseUrl() // Use the utility function

  private connection: signalR.HubConnection | null = null
  private connectionState: 'disconnected' | 'connecting' | 'connected' | 'reconnecting' =
    'disconnected'
  private lastConnectionTime: Date | null = null

  async start(): Promise<boolean> {
    if (this.connection) {
      return this.connection.state === signalR.HubConnectionState.Connected
    }

    try {
      this.connectionState = 'connecting'

      this.connection = new signalR.HubConnectionBuilder()
        .withUrl(`${this.baseUrl}taskHub`)
        .withAutomaticReconnect([0, 2000, 5000, 10000, 30000])
        .build()

      this.setupConnectionHandlers()
      this.setupMessageHandlers()

      await this.connection.start()
      this.connectionState = 'connected'

      const signalRStore = useSignalRStore()
      signalRStore.updateConnectionState('connected')

      this.lastConnectionTime = new Date()
      console.log('SignalR connection established')
      return true
    } catch (err) {
      console.error('Failed to start SignalR connection:', err)
      this.connectionState = 'disconnected'
      return false
    }
  }

  private setupConnectionHandlers(): void {
    if (!this.connection) return

    this.connection.onclose((error) => {
      console.log('SignalR connection closed:', error)
      this.connectionState = 'disconnected'
      useSignalRStore().updateConnectionState('disconnected')
    })

    this.connection.onreconnecting((error) => {
      console.log('Reconnecting due to error:', error)
      this.connectionState = 'reconnecting'
      useSignalRStore().updateConnectionState('reconnecting')
    })

    this.connection.onreconnected((connectionId) => {
      console.log('Reconnected with connection ID:', connectionId)
      this.connectionState = 'connected'
      this.lastConnectionTime = new Date()
      useSignalRStore().updateConnectionState('connected')
      this.fetchLatestTaskStatuses()
    })
  }

  private setupMessageHandlers(): void {
    if (!this.connection) return

    this.connection.on('TASK_STATUS_UPDATE', (data: TaskStatusUpdateResponse) => {
      console.log('Task status update received:', data)
      useTaskStore().updateTaskStatus(data)
    })

    this.connection.on('TASK_STATUS_UPDATES', (data: TaskStatusUpdateResponse[]) => {
      useTaskStore().updateTaskStatuses(data)
    })
  }

  async stop(): Promise<void> {
    if (this.connection) {
      await this.connection.stop()
      this.connection = null
      this.connectionState = 'disconnected'
    }
  }

  async fetchLatestTaskStatuses(): Promise<void> {
    const taskStore = useTaskStore()
    const taskKeys = Object.keys(taskStore.taskStatuses)

    if (
      taskKeys.length > 0 &&
      this.connection &&
      this.connection.state === signalR.HubConnectionState.Connected
    ) {
      try {
        await this.connection.invoke('GetLatestTaskStatuses', taskKeys)
      } catch (err) {
        console.error('Error fetching latest task statuses:', err)
      }
    }
  }

  getConnectionState(): string {
    return this.connectionState
  }

  isConnected(): boolean {
    return this.connectionState === 'connected'
  }
}

// Export singleton
export const signalRService = new SignalRService()
