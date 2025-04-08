<template>
  <v-chip :color="statusColor" :text-color="textColor" class="px-2 py-1">
    <template v-if="isProcessing">
      <v-progress-circular indeterminate size="16" width="2" color="white" class="mr-2" />
    </template>
    <span>{{ displayStatus }}</span>
  </v-chip>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { TaskStatus } from '@/models/types'

const { status } = defineProps<{
  status: TaskStatus
}>()

const isProcessing = computed(() => status === TaskStatus.Processing)

const displayStatus = computed(() => status || 'Unknown')

const statusColorMap: Record<TaskStatus, string> = {
  [TaskStatus.Queued]: 'blue',
  [TaskStatus.Processing]: 'amber',
  [TaskStatus.Completed]: 'green',
  [TaskStatus.Failed]: 'error',
}

const statusColor = computed(() => statusColorMap[status] ?? 'grey')

const textColor = computed(() =>
  [TaskStatus.Queued, TaskStatus.Processing, TaskStatus.Completed, TaskStatus.Failed].includes(
    status,
  )
    ? 'white'
    : 'black',
)
</script>

<style scoped>
.v-chip {
  display: inline-flex;
  align-items: center;
}
</style>
