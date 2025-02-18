<script setup>
import ExternalIni from "./ExternalIni.vue";
</script>
<template>
  <div v-if="!ShouldGoToPageOrsmth" style="margin-top: 20px">
    <Tabs :files="files" :activeTab="activeTab" @tab-selected="switchTab" />
    <Table :sections="currentSections" @edit="EditContent" />
  </div>

  <div v-else>
    <ExternalIni
      @back="backYKYK"
      :sections="Sections"
      :IDOfSection="activeTab"
      :PartOfSection="EditIniPart"
    />
  </div>
</template>

<script>
import Tabs from "./Tabs.vue";
import Table from "./Table.vue";
//import EditPopup from "./components/EditPopup.vue";

export default {
  components: { Tabs, Table },
  data() {
    return {
      activeTab: 0,
      ShouldGoToPageOrsmth: false,
      files: [],
      Sections: Array,
      EditIniPart: 0,
    };
  },
  computed: {
    currentSections() {
      console.log(this.files);
      // console.log(this.files[this.activeTab].Data);
      console.log(this.activeTab);
      return this.files[this.activeTab]?.Data || [];
    },
  },
  async created() {
    await this.fetchFiles();
  },
  methods: {
    backYKYK() {
      this.ShouldGoToPageOrsmth = false;
    },
    async fetchFiles() {
      const apiUrl = import.meta.env.VITE_API_URL;
      try {
        const response = await fetch(
          `${apiUrl}/admin/new/dashboard/content/dataV2/ini/1`,
          {
            method: "POST",
            headers: {
              "Content-Type": "application/json",
            },
            credentials: "include",
          }
        );
        const frfr = await response.json();
        this.files = frfr;
      } catch (error) {
        console.error("Error fetching file titles:", error);
      }
    },
    async switchTab(index) {
      this.activeTab = index;
      const fileName = this.files[index];
      console.log(this.files);
    },
    async EditContent(newData) {
      console.log(newData);
      this.EditIniPart = newData;
      const apiUrl = import.meta.env.VITE_API_URL;
      try {
        const response = await fetch(
          `${apiUrl}/admin/new/dashboard/content/data/ini/${this.activeTab}/${newData}`,
          {
            method: "POST",
            headers: {
              "Content-Type": "application/json",
            },
            credentials: "include",
          }
        );
        //console.log(await response.json());
        const frfr = await response.json();
        console.log(frfr);
        if (frfr) {
          this.Sections = frfr.Data;
          this.ShouldGoToPageOrsmth = true;
        }
      } catch (error) {
        console.error("Error fetching file titles:", error);
      }
    },
  },
};
</script>

<style>
h1 {
  text-align: center;
}
</style>
