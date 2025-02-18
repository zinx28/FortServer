<script setup lang="ts">
import { useAuthStore } from "../store";
import Nav from "./dashboarditems/Nav.vue";
import AdminPanel from "./dashboarditems/pages/AdminPanel.vue";
</script>

<template>
  <div class="layout">
    <Nav @changeTab="setTab" :NavResponse="getData" />

    <div style="flex: 1; padding: 2rem; margin-left: 250px; width: 100%">
      <AdminPanel />
    </div>
  </div>
</template>

<script lang="ts">
export default {
  data() {
    return {
      email: "",
      password: "",
      displayName: "none ggs",
      currentTab: "admin"
    };
  },
  async mounted() {
    const store = useAuthStore();
    const display = store.displayName;
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
        else this.$router.push("/dashboard");
      }
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
