// stores/sampleDataStore.ts
import { defineStore } from 'pinia'
import { api } from '@/services/api'
import type { DomainEntityDetails } from '@/models/types'

interface SampleDataState {
  entities: DomainEntityDetails[]
  loading: boolean
  error: string | null
}

export const useDataStore = defineStore('sampleData', {
  state: (): SampleDataState => ({
    entities: [],
    loading: false,
    error: null,
  }),

  getters: {
    hasEntities: (state) => state.entities.length > 0,
  },

  actions: {
    async fetchSampleData() {
      this.loading = true
      this.error = null

      try {
        // Since the API now directly returns the payload, we can assign it directly
        this.entities = await api.getSampleData()
      } catch (err) {
        this.error = err instanceof Error ? err.message : 'An unknown error occurred'
      } finally {
        this.loading = false
      }
    },
  },
})
