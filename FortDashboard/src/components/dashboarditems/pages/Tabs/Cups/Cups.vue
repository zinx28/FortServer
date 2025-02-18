<script setup>
import AddCup from "./AddCup.vue";
</script>

<template>
  <h3>Tournaments are still developement and in a unfinished state</h3>

  <div style="gap: 15px; display: flex; justify-content: center">
    <select
      style="padding: 5px; width: 180px; border-radius: 5px"
      :disabled="TourData.length === 0"
      @change="SelectedItem($event)"
    >
      <option v-if="TourData.length === 0" value="">NONE</option>
      <option
        v-else
        v-for="(tour, index) in TourData"
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
  <div
    v-if="TourData.length > 0"
    style="display: flex; justify-content: space-between; padding: 10px"
  >
    <div style="display: flex; align-items: center; flex-direction: column; gap: 15px;">
      <h4>This is still in WIP, could have issues or lack of features</h4>
      <input v-model="Title" maxlength="30" />
      <input v-model="Description" />
    </div>
    <div>
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
        <h1 style="text-align: center">{{ Title }}</h1>
      </div>
    </div>
  </div>

  <AddCup v-if="bShowEditPage" @back="BackFR" />
</template>

<style scoped>
input {
  accent-color: #4caf50;
  border: 0px;
  border-radius: 8px;
  padding: 0.5rem;
  background-color: #181717;
  transition: border 0.3s ease, background-color 0.3s ease;
}
</style>

<script>
export default {
  data() {
    return {
      TourData: [],
      bShowEditPage: false,

      // ONLY FOR VIEWING
      Title: "",
      Description: "",
    };
  },
  methods: {
    BackFR() {
      this.bShowEditPage = false;
    },
    ShowEditPage() {
      console.log("YHEHA");
      this.bShowEditPage = true;
    },
    async SelectedItem(test) {
      const selectedId = event.target.selectedIndex;
      console.log("TEST!" + selectedId);
      var Item = this.TourData[selectedId];
      const apiUrl = import.meta.env.VITE_API_URL;
      const Tourresponse2 = await fetch(
        `${apiUrl}/admin/new/dashboard/content/cups/${Item.ID}`,
        {
          method: "GET",
          headers: {
            "Content-Type": "application/json",
          },
          credentials: "include",
        }
      );
      const TourJson2 = await Tourresponse2.json();

      if (TourJson2) {
        if (TourJson2.error == false) {
          console.log(TourJson2);
          this.Title = TourJson2.body.title;
          this.Description = TourJson2.body.description;
        }
      }
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
        console.log(this.TourData[0]);

        const Tourresponse2 = await fetch(
          `${apiUrl}/admin/new/dashboard/content/cups/${this.TourData[0].ID}`,
          {
            method: "GET",
            headers: {
              "Content-Type": "application/json",
            },
            credentials: "include",
          }
        );
        const TourJson2 = await Tourresponse2.json();

        if (TourJson2) {
          if (TourJson2.error == false) {
            console.log(TourJson2);
            this.Title = TourJson2.body.title;
            this.Description = TourJson2.body.description;
          }
        }
      } else {
      }
    } catch (err) {
      console.log(err);
    }
  },
};
</script>
