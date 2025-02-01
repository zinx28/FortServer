<script setup lang="ts">
import { useAuthStore } from "../store";
import Nav from "./dashboarditems/Nav.vue";
import Content from "./dashboarditems/pages/Content.vue";
</script>

<template>
  <div class="layout">
    <Nav @changeTab="setTab" :NavResponse="getData" />

    <div style="flex: 1; padding: 2rem; margin-left: 250px; width: 100%">
      <Content />
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
      currentTab: "content",
      setup: false,
     // ContentID: this.ContentIDs
    };
  },
  async mounted() {
    //console.log("ContentID:", this.$route.params.id);
    //this.ContentID = this.$route.params.id
    //console.log(this.ContentID);
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
    setTab(tabfr: string) {
      console.log("TEST312321" + tabfr);
      if (this.currentTab != tabfr) {
        if (tabfr == "admin") this.$router.push("/dashboard/admin");
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
