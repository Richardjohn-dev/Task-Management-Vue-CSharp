<template>
  <div class="task-status">
    <v-chip :color="statusColor" :text-color="textColor" class="px-2 py-1">
      <template v-if="isProcessing">
        <v-progress-circular
          indeterminate
          size="16"
          width="2"
          color="white"
          class="mr-2"
        ></v-progress-circular>
      </template>
      <span>{{ displayStatus }}</span>
    </v-chip>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { TaskStatus } from '@/models/types'

const props = defineProps({
  status: {
    type: String,
    required: true
  },
  taskKey: {
    type: String,
    required: true
  }
})

const isProcessing = computed(() => {
  return props.status === TaskStatus.Processing
})

const displayStatus = computed(() => {
  return props.status || 'Unknown'
})

const statusColor = computed(() => {
  if (!props.status) return 'grey'

  switch (props.status) {
    case TaskStatus.Queued:
      return 'blue'
    case TaskStatus.Processing:
      return 'amber'
    case TaskStatus.Completed:
      return 'green'
    case TaskStatus.Failed:
      return 'error'
    default:
      return 'grey'
  }
})

const textColor = computed(() => {
  if (
    props.status === TaskStatus.Completed ||
    props.status === TaskStatus.Processing ||
    props.status === TaskStatus.Queued ||
    props.status === TaskStatus.Failed
  ) {
    return 'white'
  }
  return 'black'
})
</script>

<style scoped>
.task-status {
  display: inline-block;
}
</style>