<script setup lang="ts">
import Table from "./IniTable.vue";
</script>

<template>
  <!--the name of file doesnt even make sense~ legit bad with names (might tell already)-->
  <div style="margin-top: 25px">
    <button @click="$emit('back')" style="margin-bottom: 10px">Back</button>
    <button disabled style="margin-bottom: 10px">New</button>
    <button @click="SaveData">Save</button>

    <Table @edit="editfr" @update-section="trackChange" :sections="sections" />
    <!--:sections="sections"-->
  </div>
</template>

<script lang="ts">
interface SectionFrs {
  Name: string;
  Value: string | boolean | number;
}

export default {
  data() {
    return {
      DATA: Object,
      modifiedSections: new Map(),
    };
  },
  props: {
    sections: {
      type: Array as () => SectionFrs[],
      required: true,
    },
    IDOfSection: Number,
    PartOfSection: Number,
  },
  methods: {
    trackChange(index: number, value: any) {
      //console.log(typeof this.sections?.["0"].Value);
      if (typeof this.sections?.[index]?.Value === "boolean") {
        value = value === "true" ? true : value === "false" ? false : false;
      } else if (typeof this.sections?.[index]?.Value === "number") {
        value = parseInt(value, 10) || 0;
      }

      this.modifiedSections.set(index, value);
    },
    async SaveData() {
      if (this.modifiedSections.size == 0) return;

      const modifiedData = Array.from(this.modifiedSections.entries()).map(
        ([index, value]) => {
          //console.log(index, value);
          return { index, value };
        }
      );
      console.log(modifiedData);
      console.log("DATA " + this.IDOfSection);
      console.log("DATA " + this.PartOfSection);
      const apiUrl = import.meta.env.VITE_API_URL;
      try {
        // this endpoint is still like wip so ini could break or change
        const response = await fetch(
          `${apiUrl}/dashboard/v2/content/update/ini/${this.IDOfSection}/${this.PartOfSection}`,
          {
            method: "POST",
            headers: {
              "Content-Type": "application/json",
            },
            body: JSON.stringify(modifiedData),
            credentials: "include",
          }
        );

        const JsonParsed = await response.json();

        console.log(JsonParsed);
      } catch (err) {}
    },
    async editfr(frfr: string) {
      console.log(frfr);
      //const apiUrl = import.meta.env.VITE_API_URL;
    },
  },
};
</script>
