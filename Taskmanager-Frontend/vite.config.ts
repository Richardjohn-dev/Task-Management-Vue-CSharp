import { fileURLToPath, URL } from 'node:url'

import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import vueJsx from '@vitejs/plugin-vue-jsx'
import vueDevTools from 'vite-plugin-vue-devtools'

// https://vite.dev/config/
export default defineConfig({
  plugins: [vue(), vueJsx(), vueDevTools()],
  resolve: {
    alias: {
      '@': fileURLToPath(new URL('./src', import.meta.url)),
    },
  },
  server: {
    port: 3000,
    proxy: {
      '/api': {
        target: 'https://localhost:5001', // Your C# API URL
        changeOrigin: true,
        secure: false,
      },
      '/hubs': {
        target: 'https://localhost:5001', // Your SignalR hub URL
        changeOrigin: true,
        secure: false,
        ws: true,
      },
    },
  },
})
