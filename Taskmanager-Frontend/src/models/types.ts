// models/types.ts

export enum ResultStatus {
  Ok = 'Ok',
  Error = 'Error',
  NotFound = 'NotFound',
  ValidationError = 'ValidationError',
  Unauthorized = 'Unauthorized',
}

export interface EndPointResponse<T> {
  resultStatus: ResultStatus
  status: string
  success: boolean
  message: string
  errors: string[]
  payload: T
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
  // Add properties returned by your API
  taskId?: string
  enqueuedAt?: string
  // Add other properties as needed
}
