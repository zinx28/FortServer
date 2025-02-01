<script setup>
import AddCup from "./AddCup.vue";
</script>

<template>
  <h3>Cups are still developement and in a unfinished state</h3>

  <div style="gap: 15px; display: flex; justify-content: center">
    <select
      style="padding: 5px; width: 180px; border-radius: 5px"
      :disabled="TourData.length === 0"
    >
      <option v-if="TourData.length === 0" value="">NONE</option>
      <option
        v-else
        v-for="tour in TourData"
        :key="tour.id"
        :value="tour.value"
      >
        {{ tour.ID }}
      </option>
    </select>
    <button style="border-radius: 5px" @click="ShowEditPage()">New</button>
    <button class="ButtonColorType" style="border-radius: 5px">Delete</button>
  </div>
  <div
    style="margin-top: 15px; gap: 15px; display: flex; justify-content: center"
  >
    <button class="ButtonColorType" style="border-radius: 5px">
      Edit (isnt added yet)
    </button>
    <button class="ButtonColorType" style="border-radius: 5px">SOON</button>
  </div>
  <div style="display: flex; justify-content: space-between; padding: 10px">
    <h4>Viewing has been removed for now only (use old commit to view....)</h4>
    <div v-if="TourData.length > 0">
      <div
        id="DisplayItem"
        style="
          background-color: gray;
          width: 300px;
          height: 400px;
          border-radius: 10px;
          display: flex;
          text-align: center;
          align-items: end;
          justify-content: center;
          align-self: center;
        "
      >
        <h1 style="text-align: center">TEMP</h1>
      </div>
    </div>
  </div>

  <AddCup v-if="bShowEditPage" />
</template>

<script>
export default {
  data() {
    return {
      TourData: [],
      bShowEditPage: false,
    };
  },
  methods: {
    ShowEditPage() {
      console.log("YHEHA");
      this.bShowEditPage = true;
    },
  },
  async mounted() {
    const apiUrl = import.meta.env.VITE_API_URL;
    try {
      const Tourresponse = await fetch(
        `${apiUrl}/admin/new/dashboard/content/dataV2/cup/tournaments`,
        {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
          },
          credentials: "include",
        }
      );
      const TourJson = await Tourresponse.json();
      console.log(JSON.stringify(TourJson));
      if (Array.isArray(TourJson) && TourJson.length > 0) {
        this.TourData = TourJson;
        console.log("wow!");
      } else {
      }
    } catch (err) {
      console.log(err);
    }
  },
};
</script>
