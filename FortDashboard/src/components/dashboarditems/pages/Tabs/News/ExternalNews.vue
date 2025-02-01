<script setup lang="ts">
import Table from "../Ini/Table.vue";
import EditItem from "./EditItem.vue";
</script>

<template>
  <!--the name of file doesnt even make sense~ legit bad with names (might tell already)-->
  <div v-if="!ShouldShowNewsInstead" style="margin-top: 25px;">
    <button @click="$emit('back')" style="margin-bottom: 10px;">Back</button>

    <Table @edit="editfr" :sections="sections" />
  </div>

  <div v-if="ShouldShowNewsInstead">
    <EditItem 
        @back="backgrs" 
        :DATA="DATA"
        :IDOfSection="IDOfSection"
        :EditPart="EditPart"
    />
  </div>
</template>

<script lang="ts">
export default {
  data() {
    return {
      ShouldShowNewsInstead: false, // stupid ahh var name
      DATA: Object,
      EditPart: 0
    };
  },
  props: {
    sections: Array,
    IDOfSection: Number,
  },
  methods: {
    async editfr(frfr: string) {
      console.log(frfr);
      const apiUrl = import.meta.env.VITE_API_URL;

      // things
      const response = await fetch(
        `${apiUrl}/dashboard/v2/content/data/news/${this.IDOfSection}/${frfr}`,
        {
          method: "POST",
          headers: {
            "Content-Type": "application/x-www-form-urlencoded",
          },
          credentials: "include",
        }
      );

      const JsonParsed = await response.json();

      if(JsonParsed) {
        console.log(JsonParsed);
        this.DATA = JsonParsed;
        this.EditPart = parseInt(frfr, 10)
      }else {
        this.DATA = {} as any
      }

      this.ShouldShowNewsInstead = true;
    },
    backgrs() {
      this.ShouldShowNewsInstead = false;
    },
  },
};
</script>
