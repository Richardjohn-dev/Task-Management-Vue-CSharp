// stores/sampleDataStore.ts
import { defineStore } from 'pinia'
import { api } from '@/services/api'
import type { DomainEntityDetails, TaskEnqueuedResponse, EndPointResponse } from '@/models/types'
import { ResultStatus } from '@/models/types'

interface SampleDataState {
  entities: DomainEntityDetails[]
  loading: boolean
  error: string | null
  lastEnqueuedTask: TaskEnqueuedResponse | null
  enqueuingTask: boolean
  enqueueError: string | null
}

export const useSampleDataStore = defineStore('sampleData', {
  state: (): SampleDataState => ({
    entities: [],
    loading: false,
    error: null,
    lastEnqueuedTask: null,
    enqueuingTask: false,
    enqueueError: null,
  }),

  getters: {
    hasEntities: (state) => state.entities.length > 0,
  },

  actions: {
    async fetchSampleData() {
      this.loading = true
      this.error = null

      try {
        const response = await api.getSampleData()

        if (response.success) {
          this.entities = response.payload
        } else {
          this.error = response.message || 'Failed to fetch sample data'
        }
      } catch (err) {
        this.error = err instanceof Error ? err.message : 'An unknown error occurred'
      } finally {
        this.loading = false
      }
    },

    async enqueueDomainTask(details: DomainEntityDetails) {
      this.enqueuingTask = true
      this.enqueueError = null

      try {
        const response = await api.enqueueDomainTask(details)

        if (response.success) {
          this.lastEnqueuedTask = response.payload
          return true
        } else {
          this.enqueueError = response.message || 'Failed to enqueue task'
          return false
        }
      } catch (err) {
        this.enqueueError = err instanceof Error ? err.message : 'An unknown error occurred'
        return false
      } finally {
        this.enqueuingTask = false
      }
    },
  },
})
