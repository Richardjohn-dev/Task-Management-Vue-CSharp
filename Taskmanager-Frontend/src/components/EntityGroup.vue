<template>
  <v-card flat class="mb-4">
    <v-card-title class="text-subtitle-1 font-weight-bold">
      {{ groupData.group.id }}
    </v-card-title>
    <v-row cols="12">
      <v-col cols="3" v-for="entity in groupData.entities" :key="entity.item.id">
        <TaskInfo :entity="entity" />
      </v-col>
    </v-row>
  </v-card>
</template>

<script setup lang="ts">
import TaskInfo from './TaskInfo.vue'
import type { GroupItemDisplay } from '@/models/types'

defineProps<{
  groupData: GroupItemDisplay
}>()
</script>
