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
  /*components: {
    Content: defineAsyncComponent({
      loader: () => import("./dashboarditems/pages/Content.vue"),
      loadingComponent: {
        template: "<div>Loading Page</div>",
      },
      errorComponent: {
        template: "<div>rerorrs</div>",
      },
      delay: 200,
      timeout: 3000,
    }),
  },*/
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
      console.log(tabfr);
     
      console.log(this.currentTab != tabfr);
      if (this.currentTab != tabfr) {
        if (tabfr == "content") this.$router.push("/dashboard/content");
        else this.$router.push("/dashboard/admin");
      }
      this.currentTab = tabfr;
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
