<template>
  <div class="outer-class">
    <h3 style="  margin-bottom: 20px; ">Dashboard</h3>

    <!-- old ui stuff will use websockets in the future -->
    <div class="GridContainer">
      <div class="GridContent">
        <span>Online Users</span>
        <span>0</span>
      </div>
      <div class="GridContent">
        <span>Forced Season: {{ ForcedSeason }}</span>
        <span>{{ SeasonValue }} </span>
      </div>
    </div>

    <h3>Tasks... soon</h3>
    <br>
    <h5>Tables, ini buttons are incomplete and look weird</h5>
    <h5>any ui recommendations, issues please say on the discord!</h5>
  </div>
</template>

<script>
export default {
  data() {
    return {
      ForcedSeason: false,
      SeasonValue: "Forced season is recommended"
    }
  },
  async mounted() {
    const apiUrl = import.meta.env.VITE_API_URL;
    const apiResponse = await fetch(`${apiUrl}/admin/new/dashboard/panel`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      credentials: "include",
    });
    var responsefr = await apiResponse.json();
    console.log(responsefr);

    if (responsefr) {
      if (responsefr.SeasonForced) {
        this.ForcedSeason = responsefr.SeasonForced;
        this.SeasonValue = `Season: ${responsefr.Season}`
      }
    }

  }
}
</script>

<style scoped>
.outer-class {
  margin-top: 25px;
  margin-left: 30px;
  width: 100%;
  display: flex;
  flex-direction: column;
  box-sizing: border-box;
  align-items: b;
}

.GridContainer {
  display: grid;
  gap: 20px;
  width: 100%;
  grid-template-columns: repeat(auto-fill, 280px);
}

.GridContent {
  display: grid;
  align-items: center;
  justify-items: left;

  background-color: #31333500;
  width: 250px;
  border-radius: 15px;
  height: 80px;
  padding: 1rem 1rem;
  gap: 5px;
  flex-direction: column;

  border: 0.2px;
  border-style: solid;
  border-color: #6d6d6d;
}
</style>