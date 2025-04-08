<template>
  <div class="app">
    <header>
      <h1>Sample Data Application</h1>
    </header>
    <main>
      <SampleDataEntity />
    </main>
  </div>
</template>

<script setup lang="ts">
import { onMounted } from 'vue'
import SampleDataEntity from './components/SampleDataEntity.vue'
import { useSignalRStore } from '@/stores/signalRStore'

const signalRStore = useSignalRStore()

onMounted(async () => {
  console.log('App.vue Initializing SignalR connection...')
  await signalRStore.initializeConnection()
})
</script>

<style>
body {
  margin: 0;
  font-family: Arial, sans-serif;
}

.app {
  display: flex;
  flex-direction: column;
  min-height: 100vh;
}

header {
  background-color: #333;
  color: white;
  padding: 1rem;
  text-align: center;
}

main {
  flex: 1;
  padding: 1rem;
}
</style>
