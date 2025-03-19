import { createApp } from 'vue'
import './style.css'
import App from './App.vue'

createApp(App).mount('#app').$nextTick(() => {
  window.ipcRenderer.on('main-process-message', (_event, message) => {
    console.log(message)
  })
})
