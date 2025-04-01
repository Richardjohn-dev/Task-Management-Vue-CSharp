<!-- App.vue -->
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

<script lang="ts">
import { defineComponent } from 'vue'
import SampleDataEntity from './components/SampleDataEntity.vue'

export default defineComponent({
  name: 'App',
  components: {
    SampleDataEntity,
  },
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
