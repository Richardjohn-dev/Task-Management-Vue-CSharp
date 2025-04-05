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

            <v-list v-else>
              <TaskInfo v-for="entity in entities" :key="entity.item.id" :entity="entity" />
            </v-list>
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>
  </v-container>
</template>

<script lang="ts">
import { defineComponent, onMounted } from 'vue'
import { useDataStore } from '@/stores/dataStore'
import { storeToRefs } from 'pinia'
import TaskInfo from './TaskInfo.vue'

export default defineComponent({
  name: 'SampleDataEntity',
  components: {
    TaskInfo, // Register the TaskInfo component here
  },

  setup() {
    const store = useDataStore()
    const { entities, loading, error, hasEntities } = storeToRefs(store)

    const fetchData = () => {
      store.fetchSampleData()
    }

    onMounted(() => {
      fetchData()
    })

    return {
      entities,
      loading,
      error,
      hasEntities,
      fetchData,
    }
  },
})
</script>
