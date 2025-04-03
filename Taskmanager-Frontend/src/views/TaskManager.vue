<template>
  <v-app>
    <v-app-bar color="primary" density="compact">
      <v-app-bar-title>Sample Data Application</v-app-bar-title>
    </v-app-bar>

    <v-main>
      <v-container>
        <SampleDataEntity />
      </v-container>
    </v-main>
  </v-app>
</template>

<script setup lang="ts">
import SampleDataEntity from '@/components/SampleDataEntity.vue'
</script>
