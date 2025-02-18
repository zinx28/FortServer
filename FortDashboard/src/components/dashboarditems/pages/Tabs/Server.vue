<template>
  <form @submit.prevent="saveData">
    <div class="settings">
      <div class="setting">
        <span>Force Season</span>
        <input type="checkbox" v-model="settings.forceSeason" />
      </div>

      <div class="setting" v-if="settings.forceSeason">
        <span>Season</span>
        <input type="number" v-model="settings.season" />
      </div>
      <div class="setting">
        <span class="settingstitle">Weekly Quests</span>
        <span class="settingsdesc"
          >Number of weekly quests granted (like every week a new week)</span
        >
        <input
          v-if="settings.forceSeason"
          type="number"
          v-model="settings.weeklyQuest"
          :disabled="!settings.forceSeason"
          :style="{ visibility: !settings.forceSeason ? 'hidden' : 'visible' }"
        />
        <p v-if="!settings.forceSeason" class="error">
          Requires Forced Season ON
        </p>
      </div>
      <div class="setting">
        <span>Shop Rotation</span>
        <input
          v-if="settings.forceSeason"
          type="checkbox"
          v-model="settings.shopRotation"
          :disabled="!settings.forceSeason"
        />
        <p v-if="!settings.forceSeason" class="error">
          Requires Forced Season ON
        </p>
      </div>
    </div>
    <p>{{ ErrorMessage }}</p>
    <button>Save Changes</button>
  </form>
  <!-- to-do only show/enable after a change -->
  <!-- websockets might allow auto saves ???! -->
</template>

<script>
import { onMounted } from "vue";

export default {
  data() {
    return {
      settings: {
        forceSeason: false,
        season: 0,
        weeklyQuest: 1,
        shopRotation: false,
      },
      ErrorMessage: "",
    };
  },
  methods: {
    async saveData() {
      console.log("SAVE!!");
      const apiUrl = import.meta.env.VITE_API_URL;
      try {
        const response = await fetch(
          `${apiUrl}/dashboard/v2/content/update/server/1/69`, // index can be legit anything bc it isnt even a array
          {
            method: "POST",
            headers: {
              "Content-Type": "application/json",
            },
            body: JSON.stringify({
              ForcedSeason: this.settings.forceSeason,
              Season: this.settings.season,
              WeeklyQuests: this.settings.weeklyQuest,
              ShopRotation: this.settings.shopRotation,
            }),
            credentials: "include",
          }
        );
        //console.log(await response.json());
        const frfr = await response.json();
        console.log(frfr);
        if (frfr) {
          this.ErrorMessage = frfr.message;
        }
      } catch (error) {
        console.log(error);
      }
    },
  },
  async mounted() {
    const apiUrl = import.meta.env.VITE_API_URL;
    try {
      const response = await fetch(
        `${apiUrl}/dashboard/v2/content/data/server/1/69`, // index can be legit anything bc it isnt even a array
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
        this.settings.forceSeason = frfr.ForcedSeason;
        this.settings.season = frfr.SeasonForced;
        this.settings.weeklyQuest = frfr.WeeklyQuests;
        this.settings.shopRotation = frfr.ShopRotation;
      }
    } catch (err) {
      console.log(err);
    }
  },
};
</script>

<style scoped>
.settings {
  display: flex;
  flex-direction: column;
  gap: 1.25rem;
  padding: 1.5rem;
  border-radius: 10px;
  margin-top: 20px;
}

.setting {
  height: 30px;
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 0.75rem;
  background-color: #313335;
  border-radius: 8px;
  transition: transform 0.2s ease;
}

.setting:hover {
  transform: translateY(-5px);
  box-shadow: 0 4px 10px rgba(0, 0, 0, 0.15);
}
.warning,
.error {
  font-size: 0.75rem;
  color: #ffffff;
  padding: 0.5rem;
  min-width: 190px;
  max-height: 20px;
  border-radius: 8px;
}

.warning {
  background-color: #fbbf24;
}

.error {
  background-color: #ef4444;
}

input[type="checkbox"],
input[type="number"] {
  accent-color: #4caf50;
  border: 0px;
  border-radius: 8px;
  padding: 0.5rem;
  background-color: #181717;
  transition: border 0.3s ease, background-color 0.3s ease;
}

span {
  font-size: 1rem;
}

input[type="checkbox"]:disabled,
input[type="number"]:disabled {
  background-color: #161616;
}

input[type="number"]:focus {
  outline: none;
  border: 2px solid #007bff;
}
@media (max-width: 1000px) {
  .settingsdesc {
    display: none;
  }
}
</style>
