// services/api.ts
import axios from 'axios'
import type { EndPointResponse, DomainEntityDetails, TaskEnqueuedResponse } from '@/models/types'

const baseURL =
  import.meta.env.MODE === 'production'
    ? import.meta.env.VITE_PROD_API || `https://api-${window.location.hostname}/`
    : import.meta.env.VITE_DEV_API || 'https://localhost:7059/'

// Create an axios instance with base configuration
const apiClient = axios.create({
  baseURL,
  headers: {
    'Content-Type': 'application/json',
  },
})

export const api = {
  async getApiResponse<T>(endpoint: string): Promise<EndPointResponse<T>> {
    const response = await apiClient.get<EndPointResponse<T>>(endpoint)
    return response.data
  },
  async postApiResponse<TRequest, TResponse>(
    endpoint: string,
    data: TRequest,
  ): Promise<EndPointResponse<TResponse>> {
    const response = await apiClient.post<EndPointResponse<TResponse>>(endpoint, data)
    return response.data
  },

  getSampleData() {
    return this.getApiResponse<DomainEntityDetails[]>('/api/sample-data/')
  },

  enqueueDomainTask(details: DomainEntityDetails) {
    return this.postApiResponse<{ details: DomainEntityDetails }, TaskEnqueuedResponse>(
      '/api/tasks/enqueue',
      { details },
    )
  },
}
