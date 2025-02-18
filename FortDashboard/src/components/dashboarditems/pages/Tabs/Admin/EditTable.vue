<template>
  <div
    style="display: flex; width: 100%; flex-direction: column; margin-top: 25px"
  >
    <button @click="$emit('back')" style="width: 130px">Back</button>
    <br />
    <p>this page is very unfinished</p>
    <div>
      <button>Details</button>
      <button disabled>SOON</button>
    </div>
    <br />
    <div
      style="
        width: 50%;
        align-self: center;
        align-items: center;
        display: flex;
        flex-direction: column;
        gap: 20px;
      "
    >
      <input :value="DATASENT.UserData.Email" disabled />
      <div class="role-selection">
        <label>Select a Role:</label>
        <div class="radio-option">
          <input
            class="form-check-input"
            type="radio"
            name="RoleIdRadios"
            id="AdminRadio1"
            value="3"
            v-model="selectedRole"
          />
          <label class="form-check-label" for="AdminRadio1"> Admin </label>
        </div>
        <div class="radio-option">
          <input
            class="form-check-input"
            type="radio"
            name="RoleIdRadios"
            id="ModeratorRadio1"
            value="1"
            v-model="selectedRole"
          />
          <label class="form-check-label" for="ModeratorRadio1">
            Moderators
          </label>
        </div>
      </div>
      <p>{{ ErrorMessage }}</p>
      <button style="width: 200px" @click="SaveChanges">Save Changes</button>
      <button disabled style="width: 200px">Delete User</button>
    </div>
  </div>
</template>

<style scoped>
button {
  width: 300px;
}
input {
  accent-color: #4caf50;
  border: 0px;
  border-radius: 8px;
  padding: 0.5rem;
  background-color: #181717;
  transition: border 0.3s ease, background-color 0.3s ease;
}
.role-selection {
  display: flex;
  flex-direction: column;
  gap: 8px;
  font-size: 16px;
  font-weight: bold;
}
.form-check-input {
  width: 18px;
  height: 18px;
  cursor: pointer;
}
.radio-option {
  display: flex;
  align-items: center;
  gap: 8px;
  font-weight: normal;
}
</style>

<script>
export default {
  props: {
    DATASENT: Object,
  },
  data() {
    console.log(this.DATASENT?.adminInfo?.Role);
    return {
      selectedRole: this.DATASENT?.adminInfo?.Role === 3 ? "3" : "1",
      ErrorMessage: "",
    };
  },
  methods: {
    async SaveChanges() {
      const apiUrl = import.meta.env.VITE_API_URL;
      try {
        const response = await fetch(
          `${apiUrl}/admin/new/dashboard/panel/user/edit`,
          {
            method: "POST",
            headers: {
              "Content-Type": "application/json",
            },
            body: JSON.stringify({
              selectedRole: this.selectedRole,
              accountId: this.DATASENT.adminInfo.AccountId,
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
            this.DATASENT.adminInfo.Role = parseInt(this.selectedRole, 10) || 0;
            this.$emit("updatedData", this.DATASENT);
          }
        }
      } catch (error) {
        console.log(error);
      }
    },
  },
};
</script>
