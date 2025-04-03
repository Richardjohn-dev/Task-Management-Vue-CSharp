import { createRouter, createWebHistory } from 'vue-router'
import TaskManager from '../views/TaskManager.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'tasks',
      component: TaskManager,
    },
  ],
})

export default router
