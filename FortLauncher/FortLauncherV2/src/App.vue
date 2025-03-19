<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import Login from './components/Login.vue'
import Loading from './components/Loading.vue'
import Offline from './components/Offline.vue'
import { AuthData } from '../electron/types/AuthData';
import Dashboard from './components/Main/Dashboard.vue';

const status = ref<{ status: string } | null>(null);
const statusValue = computed(() => status.value?.status || 'loading');
const IsLoggedIn = ref<boolean | false>(false)

const LoginData = ref<AuthData>()
window.ipcRenderer.invoke('fortlauncher:ping').then(async (e) => {
  if(!e) return;
  console.log(e);
  await window.ipcRenderer.invoke('fortlauncher:login').then((LoginResponse) => {
    console.log(LoginResponse)
    LoginData.value = LoginResponse
  })
});

onMounted(() => {
  window.ipcRenderer.on('update-status', (_, Newstatus) => {
    console.log(`old ${status} / new ${Newstatus.status} status`)
    status.value = Newstatus
  })
  console.log("TEST")

  window.ipcRenderer.on('IsLoggedIn', (_, ShouldAutoLogin) => {
    IsLoggedIn.value = ShouldAutoLogin as boolean
  })

  return {
    status
  }
});
</script>

<template>
  <Offline v-if="statusValue === 'offline'" />
  <Login v-else-if="statusValue === 'online' && !IsLoggedIn" />
  <Dashboard  :LoginResponse="LoginData" v-else-if="IsLoggedIn" />
  <Loading v-else />
</template>

<style scoped></style>
