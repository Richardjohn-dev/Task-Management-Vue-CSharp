# Full-Stack Task Orchestration System

A sample project demonstrating advanced queue management with **parallel ordered processing** — a system that allows tasks to run concurrently across groups while strictly enforcing sequential execution within each group.

## 🚀 Overview

This project showcases full-stack development capabilities through the implementation of a sophisticated task orchestration system. It demonstrates expertise in both frontend architecture and backend distributed systems design.


## 📋 Project Background

This implementation was inspired by a real-world challenge I solved at a previous employer, where we needed to synchronize data across multiple systems while preventing race conditions. The production solution eventually expanded to include multiple specialized queue types with a master orchestration layer. This repository demonstrates the core pattern that formed the foundation of that larger system.


## 🧰 Tech Stack

### Frontend
- **Framework:** Vue 3 with Vite
- **Language:** TypeScript
- **State Management:** Pinia
- **UI Components:** Vuetify
- **HTTP Client:** Axios with Interceptors for unified success/error handling
- **Real-time:** SignalR

### Backend
- **Framework:** ASP.NET Core (C#)
- **Packages:** 
 - Hangfire for background job processing
 - FastEndpoints
 - Mediatr
 - SignalR for real-time communication

### Architecture
- **Pattern:** Vertical slice
- **API Integration:**
 - Consistent response structure with `ApiResponse<T>` for success and RFC 7807 `ProblemDetails` for errors
 - Global response handling with Axios interceptors

## 📸 Screenshots
> *Coming Soon*

## 🌐 Live Demo
> *Coming Soon*

## ✨ Key Features

### Backend Capabilities
- Dynamically assigns tasks to queues based on shared group keys (e.g. `IntegrationId`)
- Ensures **only one task per group is running at a time**
- Seamlessly supports tasks triggered both by the backend or from the UI
- Overflow handling for queued-up tasks when a group is already processing
- Background queue freeing and UI task state updates
- Built on top of Hangfire, but with custom queueing logic

### Frontend Experience
- Real-time task status monitoring
- Interactive queue visualization
- Task state transitions (queued, processing, complete)
- Error-aware rendering and notifications
- Responsive design for all devices

## 💡 Problem & Solution

### The Challenge
In enterprise environments, we often need to process background synchronization tasks triggered by scheduled services or direct user actions. These tasks frequently target shared entities (e.g., database rows), leading to potential **race conditions** and **data conflicts**.

While Hangfire provides excellent multi-queue support and per-task scoped dependencies, it has a significant limitation:

> Hangfire executes all enqueued tasks immediately — you can't control execution order within a queue. If multiple tasks target the same queue with multiple workers available, **they would all execute in parallel** — even on the same queue.

Our requirement was clear:
> Tasks within the same group must run one-by-one, but different groups should process concurrently.

### The Solution
This project implements a **custom queueing system** layered on top of Hangfire:

- 🧠 **Group Awareness**: Tasks are grouped by a key (e.g., `GroupId`)
- 🔄 **One Task at a Time per Queue**: Only one task per group is ever enqueued to Hangfire
- 📥 **Task Queues**: Remaining tasks are cached internally and tracked per group
- ✅ **Trigger Next**: When a task completes, the next in the group queue is enqueued
- 🧵 **Concurrency Control**: `SemaphoreSlim` and singleton queue services ensure thread-safe access

This architecture enables **parallel processing across groups** while maintaining **strict ordering within groups**, achieving optimal performance with data integrity guarantees.

## 🧭 System Architecture

### 1. Enqueue Flow
Shows how tasks are routed into queues depending on whether a group is already being processed.
![Enqueue Flow](images/enqueue-tasks-flow.png)

### 2. Task Processor Flow
Illustrates how each task is processed one-by-one within a group, and how the queue transitions upon completion.
![Processor Flow](images/process-tasks-flow.png)

## 🔧 Getting Started

> *Implementation details coming soon*
