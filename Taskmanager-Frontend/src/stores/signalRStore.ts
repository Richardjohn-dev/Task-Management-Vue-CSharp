// stores/signalRStore.ts
import { defineStore } from 'pinia'
import { signalRService } from '@/services/signalRService'

interface SignalRState {
  connectionState: 'disconnected' | 'connecting' | 'connected' | 'reconnecting'
  lastConnectionTime: Date | null
  connectionError: string | null
}

export const useSignalRStore = defineStore('signalR', {
  state: (): SignalRState => ({
    connectionState: 'disconnected',
    lastConnectionTime: null,
    connectionError: null,
  }),

  getters: {
    isConnected: (state) => state.connectionState === 'connected',
    getConnectionState: (state) => state.connectionState,
  },

  actions: {
    async initializeConnection() {
      console.log('Initializing SignalR connection...')
      this.connectionState = 'connecting'
      this.connectionError = null

      try {
        const success = await signalRService.start()
        if (success) {
          this.connectionState = 'connected'
          this.lastConnectionTime = new Date()
        } else {
          this.connectionState = 'disconnected'
          this.connectionError = 'Failed to establish connection'
        }
        return success
      } catch (error) {
        this.connectionState = 'disconnected'
        this.connectionError = error instanceof Error ? error.message : 'Unknown connection error'
        return false
      }
    },

    async closeConnection() {
      await signalRService.stop()
      this.connectionState = 'disconnected'
    },

    updateConnectionState(state: 'disconnected' | 'connecting' | 'connected' | 'reconnecting') {
      this.connectionState = state
      if (state === 'connected') {
        this.lastConnectionTime = new Date()
      }
    },
  },
})
