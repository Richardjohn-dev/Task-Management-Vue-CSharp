import { defineStore } from 'pinia'

export const useGlobalStore = defineStore('global', {
  state: () => ({
    isLoading: false,
    error: null as string | null,
    successMessage: null as string | null,
  }),

  getters: {
    hasError: (state) => !!state.error,
    hasSuccessMessage: (state) => !!state.successMessage,
  },

  actions: {
    setLoading(loading: boolean) {
      this.isLoading = loading
    },

    setError(error: string | null) {
      this.error = error
    },

    setSuccessMessage(message: string | null) {
      this.successMessage = message
    },
  },
})
