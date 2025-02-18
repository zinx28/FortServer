<template>
  <div class="modal" @click="$emit('back')">
    <div class="modal-header" @click.stop>
      <h2>Add a User!</h2>
      <div class="modal-body">
        <span>Add User By Discord ID</span>
        <br />
        <div
          style="
            display: flex;
            text-align: center;
            flex-direction: column;
            align-items: center;
            justify-content: center;
          "
        >
          <div class="FormGroup">
            <input
              type="text"
              name="password"
              class="FormInput"
              id="floatingInput"
              placeholder=" "
              v-model="DiscordID"
              required
            />
            <label for="floatingInput" class="FormLabel">Discord ID</label>
          </div>

          <h4>{{ ErrorMessage }}</h4>
        </div>
      </div>

      <button
        style="margin-top: 15px; margin-left: 15px"
        @click="$emit('back')"
      >
        Close
      </button>
      <button style="margin-top: 15px; margin-left: 15px" @click="AddNewUser()">
        Add
      </button>
    </div>
  </div>
</template>

<script>
export default {
  data() {
    return {
      DiscordID: "",
      ErrorMessage: "",
    };
  },
  methods: {
    async AddNewUser() {
      console.log(this.DiscordID);
      const apiUrl = import.meta.env.VITE_API_URL;
      try {
        const response = await fetch(
          `${apiUrl}/admin/new/dashboard/panel/grant`,
          {
            method: "POST",
            headers: {
              "Content-Type": "application/json",
            },
            body: JSON.stringify({
              DiscordID: this.DiscordID,
            }),
            credentials: "include",
          }
        );
        //console.log(await response.json());
        const frfr = await response.json();
        console.log(frfr);
        if (frfr) {
          this.ErrorMessage = frfr.message;
          if (frfr.error == false) {
            window.location.reload();
            //this.$emit("updatedData", this.DATASENT);
          }
        }
      } catch (error) {
        console.log(error);
      }
    },
  },
};
</script>

<style protocol>
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
.modal {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-color: rgba(0, 0, 0, 0.5);
  display: flex;
  justify-content: center;
  align-items: center;
}
.modal-body {
  display: flex;
  padding: 15px;
  width: 450px;
  height: 190px; /* gotta make this dynamic in the future! */
  background-color: #292b2c;
  flex-direction: column;
  border-radius: 20px;
}
.step-container {
  display: flex;
  flex-direction: column;
  justify-content: space-between;
  height: 100%;
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
</style>
