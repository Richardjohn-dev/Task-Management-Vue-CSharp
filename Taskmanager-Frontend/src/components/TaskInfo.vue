<template>
  <v-list-item>
    <v-card class="mb-2" width="100%">
      <v-card-subtitle>{{ entity.item.id }}</v-card-subtitle>
      <v-card-actions>
        <v-btn
          icon
          v-if="showEnqueueButton"
          size="small"
          color="primary"
          @click="enqueueTask"
          :loading="isEnqueuing"
          :disabled="isEnqueuing"
        >
          <v-icon color="green">mdi-play</v-icon>
        </v-btn>
        <DisplayTaskStatus
          v-if="taskStatus"
          :status="taskStatus"
          :taskKey="entity.item.id"
        ></DisplayTaskStatus>
      </v-card-actions>
    </v-card>
  </v-list-item>
</template>

<script setup lang="ts">
import { computed, ref } from 'vue'
import { useTaskStore } from '@/stores/taskStore'
import DisplayTaskStatus from './DisplayTaskStatus.vue'
import type { DomainEntityDetails } from '@/models/types'
import { TaskStatus } from '@/models/types'

const { entity } = defineProps<{ entity: DomainEntityDetails }>()

const taskStore = useTaskStore()

const isEnqueuing = ref(false)

const taskStatus = computed(() => {
  return taskStore.getTaskStatus(entity.taskKey.value)
})

const showEnqueueButton = computed(() => {
  return (
    !taskStatus.value ||
    taskStatus.value === TaskStatus.Completed ||
    taskStatus.value === TaskStatus.Failed
  )
})

const enqueueTask = async () => {
  isEnqueuing.value = true
  try {
    await taskStore.enqueueDomainTask(entity)
  } finally {
    isEnqueuing.value = false
  }
}
</script>
