<template>
  <v-row class="align-center" dense no-gutters style="gap: 8px; font-size: 14px">
    <v-icon :color="statusColor" size="12">mdi-circle</v-icon>
    <span>{{ connectionStatusText }}</span>
    <v-btn
      v-if="!isConnected"
      size="x-small"
      color="grey lighten-2"
      @click="reconnect"
      variant="tonal"
    >
      Reconnect
    </v-btn>
  </v-row>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useSignalRStore } from '@/stores/signalRStore'

const signalRStore = useSignalRStore()

const isConnected = computed(() => signalRStore.isConnected)

const statusColor = computed(() => {
  switch (signalRStore.connectionState) {
    case 'connected':
      return 'green'
    case 'connecting':
    case 'reconnecting':
      return 'orange'
    case 'disconnected':
      return 'red'
    default:
      return 'grey'
  }
})

const connectionStatusText = computed(() => {
  const state = signalRStore.connectionState
  switch (state) {
    case 'connected':
      return 'Connected'
    case 'connecting':
      return 'Connecting...'
    case 'reconnecting':
      return 'Reconnecting...'
    case 'disconnected':
      return 'Disconnected'
    default:
      return 'Unknown'
  }
})

const reconnect = async () => {
  console.log('Reconnecting from Status...')
  await signalRStore.initializeConnection()
}
</script>
