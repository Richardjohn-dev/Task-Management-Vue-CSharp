// services/api.ts
import axios from 'axios'
import type { EndPointResponse, DomainEntityDetails, TaskEnqueuedResponse } from '@/models/types'

// Create an axios instance with base configuration
const apiClient = axios.create({
  baseURL: 'http://localhost:5000', // Change to match your API URL
  headers: {
    'Content-Type': 'application/json',
  },
})

export const api = {
  /**
   * Generic method to get API response
   */
  async getApiResponse<T>(endpoint: string): Promise<EndPointResponse<T>> {
    const response = await apiClient.get<EndPointResponse<T>>(endpoint)
    return response.data
  },

  /**
   * Generic method to post to API
   */
  async postApiResponse<TRequest, TResponse>(
    endpoint: string,
    data: TRequest,
  ): Promise<EndPointResponse<TResponse>> {
    const response = await apiClient.post<EndPointResponse<TResponse>>(endpoint, data)
    return response.data
  },

  /**
   * Specific methods using the generic approach
   */
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
