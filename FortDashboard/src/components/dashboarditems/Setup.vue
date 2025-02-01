<template>
  <div class="modal">
    <div class="modal-header">
      <h2>Step {{ currentStep }} of ?</h2>
      <div class="modal-body">
        <div class="step-container" v-if="currentStep == 0">
          <h4>Welcome to FortDashboard</h4>
          <span>before you continue you will need to go through steps!</span>
          <span style="margin-top: auto"
            >The UI, Steps may change in the future</span
          >
        </div>
        <div class="step-container" v-if="currentStep == 1">
          <span>We detected that you are using the default login</span>
          <span>Please set the login details for 'ADMIN'</span>
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
                id="floatingInput"
                placeholder=" "
                autocomplete="fr"
                required
              />
              <label for="floatingInput" class="FormLabel">Password</label>
            </div>
            <div class="FormGroup">
              <input
                type="password"
                name="password_conf"
                v-model="password_conf"
                class="FormInput"
                id="floatingInput"
                placeholder=" "
                required
              />
              <label for="floatingInput" class="FormLabel"
                >Confirm Password</label
              >
            </div>
          </div>

          <h5>{{ ErrorMessage }}</h5>
        </div>
        <div class="step-container" v-if="currentStep == 2"></div>
      </div>

      <button style="margin-top: 15px; margin-left: 15px" @click="backStep()">
        Back
      </button>
      <button style="margin-top: 15px; margin-left: 15px" @click="nextStep()">
        {{ nextButtonText }}
      </button>
    </div>
  </div>
</template>

<script>
export default {
  data() {
    return {
      currentStep: 0,
      email: "",
      password: "",
      password_conf: "",
      ErrorMessage: "",
      nextButtonText: "Next",
    };
  },
  watch: {
    email(newv) {},
    password(newv) {
      var passwordRegex =
        /^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[@('@')$!%*?&])[A-Za-z\d@('@')$!%*?&'^\-_\#]{7,}$/;
      if (!passwordRegex.test(this.password)) {
        this.ErrorMessage =
          "Password must be at least 7 characters long and contain at least one uppercase letter, one lowercase letter, one number, and one special character.";
      } else {
        console.log(this.password);
        console.log("ds " + this.password_conf);
        if (this.password != this.password_conf) {
          this.ErrorMessage = "Password doesn't match";
        } else {
          this.ErrorMessage = "";
        }
      }
    },
    password_conf(newv) {
      if (this.password != this.password_conf) {
        this.ErrorMessage = "Password doesn't match";
      } else {
        this.ErrorMessage = "";
      }
    },
  },
  methods: {
    backStep() {
      // nothing special we just legit go back... no endpoints are sent till after setup
      if (this.currentStep != 0) {
        this.currentStep--;
        this.nextButtonText = "Next";
      }
    },
    async nextStep() {
      var maxstep = 1;
      if (this.currentStep < maxstep) {
        this.currentStep++;
        if (maxstep == this.currentStep) this.nextButtonText = "Done!";
      } else {
        console.log("DONE");
        var passwordRegex =
          /^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[@('@')$!%*?&])[A-Za-z\d@('@')$!%*?&'^\-_\#]{7,}$/;
        if (!passwordRegex.test(this.password)) {
          this.ErrorMessage =
            "Password must be at least 7 characters long and contain at least one uppercase letter, one lowercase letter, one number, and one special character.";
        } else {
          this.ErrorMessage = "";
          const apiUrl = import.meta.env.VITE_API_URL;
          try {
            const response = await fetch(`${apiUrl}/admin/new/login/setup`, {
              method: "POST",
              headers: {
                "Content-Type": "application/json",
              },
              body: JSON.stringify({
                 Email: this.email,
                 Password: this.password,
                 Password_Cn: this.password_conf
              }),
              credentials: "include",
            });

            const ResponseDATA = await response.json();

            if(ResponseDATA) {
               if(ResponseDATA.message) {
                  this.ErrorMessage = ResponseDATA.message;
               }

               if(ResponseDATA.login) {
                  window.location.reload(); // for now
               }
            }

          } catch (err) {
            console.log(err);
          }
        }
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
  height: 330px; /* gotta make this dynamic in the future! */
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
