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
  item: GroupItem
  group: SharedGroupIdentifier
  taskKey: TaskKey
}

// Represents a domain entity with a unique identifier and group identifier

// export interface TaskEnqueuedResponse {
//   taskId: string
//   enqueuedAt?: string
// }

export interface GroupItems {
  group: SharedGroupIdentifier
  items: GroupItem[]
}

export interface GroupItem {
  id: string
}
export interface SharedGroupIdentifier {
  id: string
}

export interface TaskKey {
  value: string
}

export interface TaskStatusUpdateResponse {
  taskKey: TaskKey
  status: TaskStatus
}

export enum TaskStatus {
  Queued = 'Queued',
  Processing = 'Processing',
  Completed = 'Completed',
  Failed = 'Failed',
}
