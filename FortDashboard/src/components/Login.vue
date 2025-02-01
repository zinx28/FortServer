<script setup lang="ts">
import { ref } from "vue";

const isChecked = ref(false);

const toggleCheckbox = () => {
  isChecked.value = !isChecked.value;
  console.log("Checkbox state:", isChecked.value);
};

// since this is called every time this endpoint is called we will just login/ or what ever
</script>

<template>
  <div style=" display: flex;  width: 100%; height: 100vh; align-items: center; justify-content: center;">
    <form @submit.prevent="login" class="FormClass">
    <h1 class="LoginTitle">Please sign in</h1>

    <div class="FormGroup">
      <input
        type="email"
        name="email"
        v-model="email"
        class="FormInput"
        id="floatingInput"
        placeholder=" "
        autocomplete="email"
        required
      />
      <label for="floatingInput" class="FormLabel">Email address</label>
    </div>

    <div class="FormGroup">
      <input
        type="password"
        name="password"
        v-model="password"
        class="FormInput"
        id="floatingPassword"
        placeholder=" "
        autocomplete="current-password"
        required
      />
      <label for="floatingPassword" class="FormLabel">Password</label>
    </div>

    <div class="FormCheck">
      <input
        type="checkbox"
        v-model="isChecked"
        id="customCheckbox"
        class="HiddenCheckbox"
      />
      <span
        class="CustomCheckbox"
        :class="{ checked: isChecked }"
        @click="toggleCheckbox"
      ></span>
      <label style="margin-left: 10px; font-size: 1rem" for="customCheckbox">
        Remember me
      </label>
    </div>

    <button class="SubmitButton" type="submit">Sign in</button>
    <div class="" id="error-text">
      {{ ErrorMessage }}
    </div>
    <p class="">Any issues, suggestions join the discord!</p>
  </form>
  </div>
</template>

<style scoped>
.FormClass {
  display: flex;
  text-align: center;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  width: 300px;
}
.LoginTitle {
  font-size: 30px;
  width: 100%;
}
.FormInput {
  padding: 20px 12px 8px;
  border: 0;
  width: 90%;
  border-radius: 6px;
  font-size: 1rem;
  line-height: 1;
  border: 2px solid transparent;
  position: relative;
  transition: border 0.5s ease;
}

.FormLabel {
  position: absolute;
  top: 50%;
  left: 15px;
  transform: translateY(-50%);
  color: gray;
  font-size: 1rem;
  transition: all 0.2s ease-in-out;
  pointer-events: none;
}

.FormInput:hover {
  border: 2px solid;
  border-color: #007bff;
}
.FormInput:focus {
  outline: none;
  border-color: #0953a1;
}

.FormInput:focus + .FormLabel,
.FormInput:not(:placeholder-shown) + .FormLabel {
  top: 15px;
  left: 15px;
  font-size: 0.8rem;
  border-color: #007bff;
}

.FormGroup {
  position: relative;
  width: 100%;
  margin-bottom: 10px;
}

.FormCheck {
  display: flex;
  align-items: center;
  cursor: pointer;
  width: 100%;
  margin-bottom: 10px;
}

.HiddenCheckbox {
  display: none;
}

.HiddenCheckbox:checked + .CustomCheckbox {
  background-color: #313335;
}

.HiddenCheckbox:checked + .CustomCheckbox::after {
  content: "";
  position: absolute;
  left: 5px;
  top: 2px;
  width: 6px;
  height: 10px;
  border: solid white;
  border-width: 0 2px 2px 0;
  transform: rotate(45deg);
}

.CustomCheckbox {
  width: 20px;
  height: 20px;
  border: 2px solid #ccc;
  border-radius: 4px;
  display: inline-block;
  position: relative;
  transition: all 0.2s ease;
}

.CustomCheckbox:hover {
  border-color: #007bff;
}

.SubmitButton {
  position: relative;
  width: 100%;
}
</style>

<script lang="ts">
export default {
  data() {
    return {
      email: "",
      password: "",
      ErrorMessage: ""
    };
  },
  methods: {
    async login(event: Event) {
      const apiUrl = import.meta.env.VITE_API_URL;
      console.log(apiUrl);

      const form = event.target as HTMLFormElement;
      const formData = new FormData(form);
      const data = new URLSearchParams(formData as any);

      console.log("URLSearchParams:", data.toString());

      const response = await fetch(`${apiUrl}/admin/new/login`, {
        method: "POST",
        headers: {
          "Content-Type": "application/x-www-form-urlencoded",
        },
        body: data.toString(),
        credentials: "include",
      });

      const JsonParsed = await response.json();

      console.log(JsonParsed.Token);
      // const JsonParsed = JSON.parse(result);

      if (JsonParsed) {
        if(!JsonParsed.error) {
          console.log("T " + (await this.$cookies.get("AuthToken")));
          window.location.reload();
        }else {
          this.ErrorMessage = JsonParsed.message;
        }
        console.log(JsonParsed.error);
        // secure for prod?
        console.log("T " + (await this.$cookies.get("AuthToken")));
        // this.$cookies.set("AuthToken", JsonParsed.Token, "1h", "/", "127.0.0.1"); // testing  ykyky
        //document.cookie = `AuthToken=${JsonParsed.Token}; Secure=true; path=/; SameSite=Lax`; // 1:1 trust
       // window.location.reload(); // ill do smth else at some point
        
      }
    },
  },
  async mounted() {
    // why did fantum! BECOME
  },
};
</script>
