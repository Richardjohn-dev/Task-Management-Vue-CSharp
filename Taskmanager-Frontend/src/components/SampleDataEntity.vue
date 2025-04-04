<template>
  <v-container>
    <v-card>
      <v-list v-if="hasEntities">
        <v-list-item v-for="entity in entities" :key="entity.id.value">
          <v-card class="mb-2">
            <v-card-title>{{ entity.id }}</v-card-title>
            <v-card-subtitle>Group: {{ entity.groupId }}</v-card-subtitle>
            <v-card-actions>
              <v-btn
                size="small"
                color="primary"
                @click="enqueueTask(entity)"
                :loading="enqueuingTask"
                :disabled="enqueuingTask"
              >
                Enqueue Task
              </v-btn>
            </v-card-actions>
          </v-card>
        </v-list-item>
      </v-list>
    </v-card>

    <v-row>
      <v-col>
        <v-card>
          <v-card-title class="text-h5"> Sample Data Entities </v-card-title>

          <v-card-text>
            <v-alert v-if="error" type="error" closable>
              {{ error }}
              <template v-slot:append>
                <v-btn color="error" variant="text" @click="fetchData"> Retry </v-btn>
              </template>
            </v-alert>

            <div v-if="loading" class="d-flex justify-center my-4">
              <v-progress-circular indeterminate color="primary"></v-progress-circular>
            </div>

            <v-alert v-else-if="!hasEntities" type="info">
              No sample data available.
              <template v-slot:append>
                <v-btn color="primary" variant="text" @click="fetchData"> Refresh </v-btn>
              </template>
            </v-alert>

            <v-list v-else>
              <v-list-item v-for="entity in entities" :key="entity.id.value">
                <v-card class="mb-2">
                  <v-card-title>Id: {{ entity.id.value }}</v-card-title>
                  <v-card-subtitle>Group: {{ entity.groupId }}</v-card-subtitle>
                  <v-card-actions>
                    <v-btn
                      size="small"
                      color="primary"
                      @click="enqueueTask(entity)"
                      :loading="enqueuingTask"
                      :disabled="enqueuingTask"
                    >
                      Enqueue Task
                    </v-btn>
                  </v-card-actions>
                </v-card>
              </v-list-item>
            </v-list>
            <!-- <v-table v-else hover density="comfortable" class="mt-4">
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
                    <v-btn
                      size="small"
                      color="primary"
                      @click="enqueueTask(entity)"
                      :loading="enqueuingTask"
                      :disabled="enqueuingTask"
                    >
                      Enqueue Task
                    </v-btn>
                  </td>
                </tr>
              </tbody>
            </v-table> -->
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>

    <v-row v-if="lastEnqueuedTask">
      <v-col>
        <v-card color="success" class="mt-4">
          <v-card-title class="text-h6"> Last Enqueued Task </v-card-title>
          <v-card-text>
            <v-sheet class="bg-surface-variant pa-4 rounded" elevation="1">
              <pre>{{ JSON.stringify(lastEnqueuedTask, null, 2) }}</pre>
            </v-sheet>
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>

    <v-row v-if="enqueueError">
      <v-col>
        <v-alert type="error" class="mt-4"> Error enqueuing task: {{ enqueueError }} </v-alert>
      </v-col>
    </v-row>
  </v-container>
</template>

<script lang="ts">
import { defineComponent, onMounted } from 'vue'
import { useDataStore } from '@/stores/dataStore'
import { storeToRefs } from 'pinia'
import type { DomainEntityDetails } from '@/models/types'

export default defineComponent({
  name: 'SampleDataEntity',

  setup() {
    const store = useDataStore()
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
/* Custom styling can be added here if needed */
pre {
  white-space: pre-wrap;
  word-break: break-word;
}
</style>
