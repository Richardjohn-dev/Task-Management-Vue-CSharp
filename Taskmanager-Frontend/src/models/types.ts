// models/types.ts

export interface ProblemDetailError {
  name: string
  reason: string
}

export interface ProblemDetails {
  type: string
  title: string
  status: number
  instance: string
  traceId: string
  errors?: ProblemDetailError[]
}
export interface ApiResponse<T> {
  payload?: T
  pagination?: Pagination
}

export interface Pagination {
  records: number
  pageNumber: number
  pageSize: number
  totalPages: number
  nextPage: string
  previousPage: string
  nextCursor?: string
  previousCursor?: string
}

export interface DomainEntityDetails {
  id: SomeItemInfo
  groupId: SharedGroupIdentifier
}

// Represents a domain entity with a unique identifier and group identifier
export interface SomeItemInfo {
  value: string
}

export interface SharedGroupIdentifier {
  value: string
}

export interface TaskEnqueuedResponse {
  taskId: string
  enqueuedAt?: string
}
