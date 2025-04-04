// // stores/sampleDataStore.ts
// import { defineStore } from 'pinia'
// import { api } from '@/services/api'
// import type { DomainEntityDetails, TaskEnqueuedResponse } from '@/models/types'

// interface SampleDataState {
//   entities: DomainEntityDetails[]
//   loading: boolean
//   error: string | null
//   lastEnqueuedTask: TaskEnqueuedResponse | null
//   enqueuingTask: boolean
//   enqueueError: string | null
// }

// export const useDataStore = defineStore('sampleData', {
//   state: (): SampleDataState => ({
//     entities: [],
//     loading: false,
//     error: null,
//     lastEnqueuedTask: null,
//     enqueuingTask: false,
//     enqueueError: null,
//   }),

//   getters: {
//     hasEntities: (state) => state.entities.length > 0,
//   },

//   actions: {
//     async fetchSampleData() {
//       this.loading = true
//       this.error = null

//       try {
//         const response = await api.getSampleData()
//         console.log('get api response: ', response)
//         if (response.success) {
//           this.entities = response.payload
//         } else {
//           this.error = response.message || 'Failed to fetch sample data'
//         }
//       } catch (err) {
//         this.error = err instanceof Error ? err.message : 'An unknown error occurred'
//       } finally {
//         this.loading = false
//       }
//     },

//     async enqueueDomainTask(details: DomainEntityDetails) {
//       this.enqueuingTask = true
//       this.enqueueError = null

//       try {
//         const response = await api.enqueueDomainTask(details)

//         if (response.success) {
//           this.lastEnqueuedTask = response.payload
//           return true
//         } else {
//           this.enqueueError = response.message || 'Failed to enqueue task'
//           return false
//         }
//       } catch (err) {
//         this.enqueueError = err instanceof Error ? err.message : 'An unknown error occurred'
//         return false
//       } finally {
//         this.enqueuingTask = false
//       }
//     },
//   },
// })
// stores/sampleDataStore.ts
import { defineStore } from 'pinia'
import { api } from '@/services/api'
import type { DomainEntityDetails, TaskEnqueuedResponse } from '@/models/types'

interface SampleDataState {
  entities: DomainEntityDetails[]
  loading: boolean
  error: string | null
  lastEnqueuedTask: TaskEnqueuedResponse | null
  enqueuingTask: boolean
  enqueueError: string | null
}

export const useDataStore = defineStore('sampleData', {
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
        // Since the API now directly returns the payload, we can assign it directly
        this.entities = await api.getSampleData()
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
        // API now returns the payload directly
        // what if this fails, what are we doing here?
        this.lastEnqueuedTask = await api.enqueueDomainTask(details)
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
