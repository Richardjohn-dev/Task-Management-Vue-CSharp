<!-- components/SampleDataEntity.vue -->
<template>
  <div class="sample-data-container">
    <h2>Sample Data Entities</h2>

    <div v-if="loading" class="loading">Loading sample data...</div>

    <div v-else-if="error" class="error">
      <p>Error: {{ error }}</p>
      <button @click="fetchData" class="retry-button">Retry</button>
    </div>

    <div v-else-if="!hasEntities" class="no-data">
      <p>No sample data available.</p>
      <button @click="fetchData" class="retry-button">Refresh</button>
    </div>

    <div v-else class="data-table">
      <table>
        <thead>
          <tr>
            <th>ID</th>
            <th>Group ID</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="entity in entities" :key="entity.id.value">
            <td>{{ entity.id.value }}</td>
            <td>{{ entity.groupId.value }}</td>
            <td>
              <button @click="enqueueTask(entity)" :disabled="enqueuingTask" class="enqueue-button">
                Enqueue Task
              </button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <div v-if="lastEnqueuedTask" class="task-enqueued">
      <h3>Last Enqueued Task:</h3>
      <pre>{{ JSON.stringify(lastEnqueuedTask, null, 2) }}</pre>
    </div>

    <div v-if="enqueueError" class="enqueue-error">
      <p>Error enqueuing task: {{ enqueueError }}</p>
    </div>
  </div>
</template>

<script lang="ts">
import { defineComponent, onMounted } from 'vue'
import { useSampleDataStore } from '@/stores/sampleDataStore'
import { storeToRefs } from 'pinia'
import type { DomainEntityDetails } from '@/models/types'

export default defineComponent({
  name: 'SampleDataEntity',

  setup() {
    const store = useSampleDataStore()
    const { entities, loading, error, lastEnqueuedTask, enqueuingTask, enqueueError, hasEntities } =
      storeToRefs(store)

    const fetchData = () => {
      store.fetchSampleData()
    }

    const enqueueTask = async (entity: DomainEntityDetails) => {
      await store.enqueueDomainTask(entity)
    }

    onMounted(() => {
      fetchData()
    })

    return {
      entities,
      loading,
      error,
      lastEnqueuedTask,
      enqueuingTask,
      enqueueError,
      hasEntities,
      fetchData,
      enqueueTask,
    }
  },
})
</script>

<style scoped>
.sample-data-container {
  padding: 20px;
  max-width: 1000px;
  margin: 0 auto;
}

.data-table {
  margin-top: 20px;
}

table {
  width: 100%;
  border-collapse: collapse;
}

th,
td {
  padding: 8px 12px;
  text-align: left;
  border-bottom: 1px solid #ddd;
}

th {
  background-color: #f2f2f2;
}

.enqueue-button {
  background-color: #4caf50;
  color: white;
  padding: 6px 12px;
  border: none;
  border-radius: 4px;
  cursor: pointer;
}

.enqueue-button:disabled {
  background-color: #cccccc;
  cursor: not-allowed;
}

.loading,
.error,
.no-data,
.task-enqueued,
.enqueue-error {
  margin: 20px 0;
  padding: 15px;
  border-radius: 4px;
}

.loading {
  background-color: #f8f9fa;
  color: #6c757d;
}

.error,
.enqueue-error {
  background-color: #f8d7da;
  color: #721c24;
}

.retry-button {
  margin-top: 10px;
  padding: 6px 12px;
  background-color: #007bff;
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
}

.task-enqueued {
  background-color: #d4edda;
  color: #155724;
}

pre {
  background-color: #f8f9fa;
  padding: 10px;
  border-radius: 4px;
  white-space: pre-wrap;
}
</style>
