// SampleDataEntity.vue
<template>
  <v-container>
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

            <v-row v-for="groupData in groupedEntities" :key="groupData.group.id" cols="12" v-else>
              <EntityGroup :groupData="groupData" />
            </v-row>
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>
  </v-container>
</template>

<script setup lang="ts">
import { computed, onMounted } from 'vue'
import { useDataStore } from '@/stores/dataStore'
import { storeToRefs } from 'pinia'
import EntityGroup from './EntityGroup.vue'
import type { GroupItemDisplay } from '@/models/types'

const store = useDataStore()
const { entities, loading, error, hasEntities } = storeToRefs(store)

const groupedEntities = computed<GroupItemDisplay[]>(() => {
  // Create a map to store entities by group ID
  const groupMap = new Map<string, GroupItemDisplay>()

  // Populate the map
  entities.value.forEach((entity) => {
    const groupId = entity.group.id

    if (!groupMap.has(groupId)) {
      groupMap.set(groupId, {
        group: entity.group,
        entities: [],
      })
    }

    groupMap.get(groupId)?.entities.push(entity)
  })

  // Convert map to array
  return Array.from(groupMap.values())
})

const fetchData = () => {
  store.fetchSampleData()
}

onMounted(() => {
  fetchData()
})
</script>
