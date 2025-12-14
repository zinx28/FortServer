<script setup lang="ts">
import About from '../Settings/About.vue';
import Account from '../Settings/Account.vue';
import Developer from '../Settings/Developer.vue';

</script>
<template>
  <div class="CenterContent">
    <a style="color: white; font-size: 22px; font-weight: bold; text-align: left;  width: 95%;">Settings, unfinished</a>

    <div class="btn-grid">
      <button class="btn" @click="setTab('account')" :class="{ active: currentTab === 'account' }">Account</button>
      <button class="btn" @click="setTab('launcher')" :class="{ active: currentTab === 'launcher' }">Launcher</button>
      <button class="btn" @click="setTab('developer')"
        :class="{ active: currentTab === 'developer' }">Developer</button>
      <button class="btn" @click="setTab('about')" :class="{ active: currentTab === 'about' }">About</button>
    </div>


    <div v-if="currentTab === 'account'" class="tab-wrapper">
      <Account :LoginResponse="user" />
    </div>

    <div v-if="currentTab === 'developer'" class="tab-wrapper">
      <Developer />
    </div>


    <div v-if="currentTab === 'about'" class="tab-wrapper">
      <About />
    </div>


  </div>

</template>

<script lang="ts">

export default {
  data() {
    return {
      currentTab: 'account',
      TabName: 'account',
    }
  },
  props: {
    LoginResponse: {
      type: Object,
      default: () => ({})
    }
  },
  computed: {
    user() {
      return this.LoginResponse
    }
  },
  methods: {
    setTab(tab: string) {
      if (tab != this.TabName) {
        this.TabName = tab
        this.currentTab = tab
      }
    },
    async openAppData() {
      try {
        await window.ipcRenderer.invoke("fortlauncher:open-appdata");
      } catch (err) {
        console.error("Failed to open AppData:", err);
      }
    },
    async LogoutLauncher() {
      try {
        await window.ipcRenderer.invoke("fortlauncher:logout");
      } catch (err) {
        console.error("Failed to logout:", err);
      }
    }
  }
}
</script>

<style scoped>
.tab-wrapper {
  width: 90%;
  display: flex;
  flex-direction: column;
  justify-content: flex-start;
  align-items: center;
}

.btn-grid {
  background-color: #1b1b1b;
  width: 90%;
  height: 45px;
  border-radius: 10px;
  display: flex;
  justify-content: space-around;
  align-items: center;
  padding: 0 6px;
  gap: 6px;
}

.btn-grid .active {
  background: #2a2a2a;
}

.btn {
  width: 85%;
  height: 32px;
  border-radius: 8px;
  color: #fff;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
}

.btn:focus {
  outline: none;
  box-shadow: none;
}

.btn:active {
  outline: none;
  box-shadow: none;
  border: 0;
}

.open-appdata {
  margin-top: 8px;
}

.CenterContent {
  margin-top: 15px;
  display: flex;
  flex-direction: column;
  width: 100%;
  align-items: center;
  gap: 15px;
}

.SomeDivINthebuild {
  display: flex;
  flex-direction: column;
  width: 100%;
  align-items: flex-start;
  margin-top: 5px;
}

.BuildImageHOlder .launchButton {
  opacity: 0;
  visibility: hidden;
  transition: opacity 0.3s ease;
}


.BuildImageHOlder:hover {
  background-color: #5f5f5ffb;
}


.BuildImageHOlder:hover .launchButton {
  opacity: 1;
  visibility: visible;
}

.launchButton.running {
  background-color: #27ae60;
}

.launchButton:disabled {
  background-color: #bdc3c7;
  cursor: not-allowed;
}

.launchButton {
  background-color: #1c1c1e;
  color: white;
  border: none;
  padding: 8px 0px;
  width: 90%;
  border-radius: 5px;
  cursor: pointer;
  font-size: 16px;
  margin-top: auto;
  margin-bottom: 10px;
}

.GridContainer {
  display: grid;
  grid-template-columns: repeat(auto-fit, 170px);
  gap: 15px;
  width: 90%;
  height: 170px;
  justify-content: start;

}

.BuildTitle {
  color: white;
  font-size: 18px;
  font-weight: bold;
  margin-top: 0px;
  margin-left: 10px;
}

.BuildBody {
  color: #9c9891;
  font-size: 14px;
  font-weight: normal;
  margin-top: 0px;
  margin-left: 10px;
}

.BuildImageHOlder {
  width: 170px;
  background-color: #a5a5a5;
  border-style: solid;
  border-width: 1px;
  border-color: #1e1e2d;
  height: 225px;
  border-radius: 10px;
  display: flex;
  justify-content: center;
  align-items: center;
  position: relative;
  overflow: hidden;
  transition: all 0.3s ease;
}


.AddBuildB {
  width: 170px;
  background-color: transparent;
  border-style: dotted;
  border-width: 2px;
  border-color: #a0a0b8;
  height: 225px;
  border-radius: 10px;
  display: flex;
  justify-content: center;
  align-items: center;
  position: relative;
  overflow: hidden;
  transition: all 0.3s ease;
  flex-direction: column;
}

.GridThing {
  display: flex;
  flex-direction: column;
  width: 170px;
  height: 290x;
  justify-content: flex-start;
  border-radius: 10px;
  align-items: flex-start;
}
</style>