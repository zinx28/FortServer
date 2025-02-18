<script setup lang="ts">
import { useAuthStore } from "../store";
import Nav from "./dashboarditems/Nav.vue";
import Home from "./dashboarditems/pages/Home.vue";
import Setup from "./dashboarditems/Setup.vue";
</script>

<template>
  <div class="layout">
    <Nav @changeTab="setTab" :NavResponse="getData" />

    <div style="flex: 1; padding: 2rem; margin-left: 250px; width: 100%">
        <Home />
    </div>
  </div>

  <Setup v-if="setup" />
</template>

<script lang="ts">
export default {
  data() {
    return {
      email: "",
      password: "",
      displayName: "none ggs",
      currentTab: "home",
      setup: false,
    };
  },
  async mounted() {
    const store = useAuthStore();
    const display = store.displayName;
    this.setup = store.setup;
    console.log("TEST " + display);
    this.displayName = display;
  },
  computed: {
    getData() {
      console.log(this.displayName);
      return {
        displayName: this.displayName,
        currentTab: this.currentTab,
      };
    },
  },
  methods: {
    setTab(selectedTab: string) {
      if (this.currentTab != selectedTab) {
        if (selectedTab == "content") this.$router.push("/dashboard/content");
        else this.$router.push("/dashboard/admin");
      }
      this.currentTab = selectedTab;
    },
  },
};
</script>

<style>
.layout {
  display: flex;
}
body {
  margin: 0;
}
</style>
