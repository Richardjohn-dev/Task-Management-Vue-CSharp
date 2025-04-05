<template>
  <v-list-item>
    <v-card class="mb-2" width="100%">
      <v-card-title>{{ entity.item.id }}</v-card-title>
      <v-card-subtitle>Group: {{ entity.group.id }}</v-card-subtitle>
      <v-card-actions>
        <v-btn
          v-if="showEnqueueButton"
          size="small"
          color="primary"
          @click="enqueueTask"
          :loading="isEnqueuing"
          :disabled="isEnqueuing"
        >
          Enqueue Task
        </v-btn>
        <TaskStatus v-if="taskStatus" :status="taskStatus" :taskKey="entity.item.id"></TaskStatus>
      </v-card-actions>
    </v-card>
  </v-list-item>
</template>

<script setup lang="ts">
import { computed, ref } from 'vue'
import { useTaskStore } from '@/stores/taskStore'
// import { storeToRefs } from 'pinia'
import TaskStatus from './TaskStatus.vue'
import type { DomainEntityDetails } from '@/models/types'
import { TaskStatus as TaskStatusEnum } from '@/models/types'

const props = defineProps<{
  entity: DomainEntityDetails
}>()

const taskStore = useTaskStore()
// const { enqueuingTask } = storeToRefs(taskStore)

// Local loading state for this specific entity
const isEnqueuing = ref(false)

// Get the task status directly as a computed property
const taskStatus = computed(() => {
  return taskStore.getTaskStatus(props.entity.taskKey.value)
})

const showEnqueueButton = computed(() => {
  return (
    !taskStatus.value ||
    taskStatus.value === TaskStatusEnum.Completed ||
    taskStatus.value === TaskStatusEnum.Failed
  )
})

const enqueueTask = async () => {
  isEnqueuing.value = true
  try {
    await taskStore.enqueueDomainTask(props.entity)
  } finally {
    isEnqueuing.value = false
  }
}
</script>
