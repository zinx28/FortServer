<script setup>
import { defineAsyncComponent } from "vue";
import ExternalNews from "./Tabs/News/ExternalNews.vue";
</script>

<template>
  <div class="tabs-container">
    <div class="tabs-header">
      <a
        :class="['tab-button', { active: activeTab === 'news' }]"
        href="/dashboard/content/news"
      >
        News Update
      </a>
      <a
        :class="['tab-button', { active: activeTab === 'server' }]"
        href="/dashboard/content/server"
      >
        Server Management
      </a>
      <a
        :class="['tab-button', { active: activeTab === 'ini' }]"
        href="/dashboard/content/ini"
      >
        Ini Management
      </a>
      <a
        :class="['tab-button', { active: activeTab === 'cup' }]"
        href="/dashboard/content/cup"
      >
        Tournaments
      </a>
    </div>
  </div>

  <div v-if="activeTab == 'news' && !bExternalNews">
    <div class="GridContainer">
      <div class="GridContent" @click="newsPage(1)">
        <span class="TitleT">News</span>
        <span>N/A</span>
      </div>
      <div class="GridContent" @click="newsPage(2)">
        <span class="TitleT">Emergency Notice</span>
        <span>N/A</span>
      </div>
      <div class="GridContent" @click="newsPage(3)">
        <span class="TitleT">Login Message</span>
        <span>N/A</span>
      </div>
      <div class="GridContent" @click="newsPage(4)">
        <span class="TitleT">Playlist Information</span>
        <span>N/A</span>
      </div>
    </div>
  </div>

  <div v-if="activeTab == 'server' && !bExternalNews">
    <Server />
  </div>

  <div v-if="activeTab == 'ini' && !bExternalNews">
    <Ini />
  </div>

  <div v-if="activeTab == 'cup' && !bExternalNews">
    <Cups />
  </div>

  <div v-if="bExternalNews">
    <ExternalNews @back="backYKYK" :sections="Sections" :IDOfSection="sectionID" />
  </div>
</template>

<script>
export default {
  data() {
    return {
      activeTab: "news",
      bExternalNews: false,
      Sections: String[0], // ill name soon
      sectionID: 1,
    };
  },
  components: {
    Server: defineAsyncComponent({
      loader: () => import("./Tabs/Server.vue"),
      loadingComponent: {
        template: "<div>Loading Page</div>",
      },
      errorComponent: {
        template: "<div>rerorrs</div>",
      },
      delay: 200,
      timeout: 3000,
    }),
    Ini: defineAsyncComponent({
      loader: () => import("./Tabs/Ini/Ini.vue"),
      loadingComponent: {
        template: "<div>Loading Page</div>",
      },
      errorComponent: {
        template: "<div>rerorrs</div>",
      },
      delay: 200,
      timeout: 3000,
    }),
    Cups: defineAsyncComponent({
      loader: () => import("./Tabs/Cups/Cups.vue"),
      loadingComponent: {
        template: "<div>Loading Page</div>",
      },
      errorComponent: {
        template: "<div>rerorrs</div>",
      },
      delay: 200,
      timeout: 3000,
    }),
  },
  props: {
    tabs: Array,
    modelValue: String,
  },
  mounted() {
    console.log(this.$route.params.id);
    if (this.$route.params.id != null) {
      this.activeTab = this.$route.params.id.toString();
    }
  },
  methods: {
    //setActiveTab(tab) {
    //if (tab != this.activeTab) {
    // this.activeTab = tab;
    // this.bExternalNews = false;
    //}
    //},
    backYKYK() {
      this.bExternalNews = false;
    },
    async newsPage(pageindex) {
      const apiUrl = import.meta.env.VITE_API_URL;
      // this was kinda rushed and half of this !!!
      console.log(`${apiUrl}/dashboard/v2/content/id/news/${pageindex}`);
      const response = await fetch(
        `${apiUrl}/dashboard/v2/content/id/news/${pageindex}`,
        {
          method: "POST",
          headers: {
            "Content-Type": "application/x-www-form-urlencoded",
          },
          credentials: "include",
        }
      );

      const JsonParsed = await response.json();

      console.log(JsonParsed);

      if (Array.isArray(JsonParsed)) {
        this.Sections = JsonParsed;
        this.sectionID = pageindex;
        console.log("E + " + this.Sections);
      }

      this.bExternalNews = true;
    },
  },
  beforeUnmount() {
    //if (this.interval) {
    //clearInterval(this.interval);
    //}
  },
};
</script>

<style scoped>
/* slit for diff page (remove when implemnting apis!) */
.GridContainer {
  margin-top: 20px;
  display: grid;
  gap: 20px;
  width: 100%;
  grid-template-columns: repeat(auto-fill, 250px);
}
.GridContent {
  display: grid;
  text-align: start;
  width: 220px;
  height: 70px;
  padding: 20px;
  border-radius: 20px;
  background-color: #313335;
  transition: transform 0.2s ease;
}
.GridContent:hover {
  transform: translateY(-5px);
  box-shadow: 0 4px 10px rgba(0, 0, 0, 0.15);
}

.TitleT {
  font-size: 16px;
  font-weight: bold;
}

.tabs-container {
  display: flex;
  border-bottom: 1px solid #e0e0e0;
  padding: 8px;
}

.tabs-header {
  display: flex;
  gap: 8px;
}

.tab-button {
  padding: 8px 16px;
  border: none;
  background: transparent;
  font-size: 14px;
  font-weight: 500;
  color: #ffffff;
  border-radius: 8px;
  cursor: pointer;
  transition: background 0.2s, color 0.2s;
}

.tab-button.active {
  background: #f5f5f5;
  color: #000;
  font-weight: bold;
}
</style>
