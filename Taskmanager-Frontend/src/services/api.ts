// services/api.ts
import axios, { AxiosError } from 'axios'
import type {
  ApiResponse,
  DomainEntityDetails,
  ProblemDetails,
  TaskStatusUpdateResponse,
} from '@/models/types'
import { useGlobalStore } from '@/stores/globalStore'
import { getBaseUrl } from '@/utils/baseUrl'

// Create an axios instance with base configuration
const apiClient = axios.create({
  baseURL: getBaseUrl(),
  headers: {
    'Content-Type': 'application/json',
  },
})

function isProblemDetails(data: unknown): data is ProblemDetails {
  return (
    typeof data === 'object' &&
    data !== null &&
    'title' in data &&
    'type' in data &&
    'status' in data
  )
}

apiClient.interceptors.response.use(
  (response) => {
    // Since ApiResponse now inherently means success, we don't need to check a success flag
    return response
  },
  (error: AxiosError) => {
    const globalStore = useGlobalStore()
    const data = error.response?.data

    if (isProblemDetails(data)) {
      const problem = data
      // Create a single error message combining title and error details
      let errorMessage = problem.title

      // Add any detailed errors
      if (problem.errors && problem.errors.length > 0) {
        const errorDetails = problem.errors.map((err) => `${err.name}: ${err.reason}`).join('; ')

        if (errorDetails) {
          errorMessage += `: ${errorDetails}`
        }
      }

      globalStore.setError(errorMessage)
    } else {
      globalStore.setError(error.message || 'Unknown network error')
    }

    return Promise.reject(error)
  },
)

// Type for empty responses
type EmptyResponse = Record<string, never>

export const api = {
  async get<T>(endpoint: string): Promise<T> {
    const response = await apiClient.get<ApiResponse<T>>(endpoint)
    return response.data.payload as T
  },

  async post<TRequest, TResponse>(endpoint: string, data: TRequest): Promise<TResponse> {
    const response = await apiClient.post<ApiResponse<TResponse>>(endpoint, data)
    return response.data.payload as TResponse
  },

  // Method for operations that don't return meaningful data
  async update<TRequest>(endpoint: string, data: TRequest): Promise<void> {
    await apiClient.put<ApiResponse<EmptyResponse>>(endpoint, data)
    // No return value needed - just resolves if successful, rejects if error
  },

  async delete(endpoint: string): Promise<void> {
    await apiClient.delete<ApiResponse<EmptyResponse>>(endpoint)
    // No return value needed - just resolves if successful, rejects if error
  },

  getSampleData() {
    return this.get<DomainEntityDetails[]>('/api/sample-data/')
  },

  enqueueDomainTask(details: DomainEntityDetails) {
    return this.post<{ details: DomainEntityDetails }, TaskStatusUpdateResponse>(
      '/api/tasks/enqueue',
      {
        details,
      },
    )
  },

  // Example of an update operation that doesn't return data
  updateEntity(id: string, updatedData: Partial<DomainEntityDetails>) {
    return this.update(`/api/entities/${id}`, updatedData)
  },
}
