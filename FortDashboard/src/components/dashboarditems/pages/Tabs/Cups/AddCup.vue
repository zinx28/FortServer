<template>
  <div class="modal" @click="$emit('back')">
    <div class="modal-header" @click.stop>
      <h2>Add a Tournaments! Step {{ CurrentStep }} of 3</h2>
      <form id="CupID">
        <div v-if="CurrentStep == 0" class="modal-body">
          <span>Tournaments are still WIP</span>
          <span
            >By default the tournaments are private, you will have a choice to
            make it public after!</span
          >
          <span
            >The setup goes through the basic stuff, more advanced stuff will be
            available after</span
          >
        </div>
        <div v-if="CurrentStep == 1" class="modal-body">
          <span>Let's set Title and Description</span>
          <div class="FormGroup">
            <input
              type="text"
              name="title"
              v-model="title"
              class="FormInput"
              id="floatingInput"
              placeholder=" "
              required
            />
            <label for="floatingInput" class="FormLabel">Title</label>
          </div>
          <div class="FormGroup">
            <input
              type="text"
              name="description"
              v-model="description"
              class="FormInput"
              id="floatingInput"
              placeholder=" "
              required
            />
            <label for="floatingInput" class="FormLabel">Description</label>
          </div>
        </div>
        <div v-if="CurrentStep == 2" class="modal-body">
          <span>Does this date look correct?</span>
          <div class="FormGroup">
            <input
              type="datetime-local"
              name="StartTime"
              v-model="StartTime"
              class="FormInput"
              id="floatingInput"
              placeholder=" "
              required
            />
            <label for="floatingInput" class="FormLabel">Start Time</label>
          </div>
          <div class="FormGroup">
            <input
              type="datetime-local"
              name="EndTime"
              v-model="EndTime"
              class="FormInput"
              id="floatingInput"
              placeholder=" "
              required
            />
            <label for="floatingInput" class="FormLabel">End Time</label>
          </div>
        </div>
        <div v-if="CurrentStep == 3" class="modal-body">
          <div class="FormGroup">
            <input
              type="text"
              name="GivenItem"
              v-model="GivenItem"
              class="FormInput"
              id="floatingInput"
              placeholder=" "
              required
            />
            <label for="floatingInput" class="FormLabel">Item To Give</label>
          </div>
          <div class="FormGroup">
            <input
              type="number"
              min="1"
              name="ItemQuantity"
              v-model="TheAmount"
              class="FormInput"
              id="floatingInput"
              placeholder=" "
              required
            />
            <label for="floatingInput" class="FormLabel">The Amount</label>
          </div>
          <div class="FormGroup">
            <input
              type="text"
              min="1"
              name="CupPlacement"
              v-model="CupPlacement"
              class="FormInput"
              id="floatingInput"
              placeholder=" "
              required
            />
            <label for="floatingInput" class="FormLabel">Given Placement</label>
          </div>
        </div>
      </form>

      <h5>{{ ErrorMessage }}</h5>
      <button style="margin-top: 15px; margin-left: 15px" @click="BackStep()">
        {{ backButtonText }}
      </button>
      <button style="margin-top: 15px; margin-left: 15px" @click="NextStep()">
        {{ nextButtonText }}
      </button>
    </div>
  </div>
</template>

<script>
import { onMounted } from "vue";

export default {
  data() {
    return {
      title: "My First Cup!",
      description: "",
      StartTime: "",
      EndTime: "",
      GivenItem: "Currency:MtxPurchased",
      TheAmount: 1000,
      CupPlacement: 50,
      ErrorMessage: "",
      CurrentStep: 0,
      backButtonText: "Close",
      nextButtonText: "Next",
    };
  },
  methods: {
    BackStep() {
      // nothing special we just legit go back... no endpoints are sent till after setup
      if (this.CurrentStep != 0) {
        this.CurrentStep--;
        this.nextButtonText = "Next";
        this.backButtonText = "Back";
        if (this.CurrentStep == 0) {
          this.backButtonText = "Close";
        }
      } else {
        this.$emit("back");
      }
      //        @click="$emit('back')"
    },
    async NextStep() {
      var maxstep = 3;
      if (this.CurrentStep < maxstep) {
        this.CurrentStep++;
        this.backButtonText = "Back";
        if (maxstep == this.CurrentStep) this.nextButtonText = "Create";
      } else {
        console.log("SAVE!");
        const apiUrl = import.meta.env.VITE_API_URL;
        try {
          const formDataObj = {
            title: this.title || "My First Cup!",
            description: this.description || "",
            StartTime: this.StartTime,
            EndTime: this.EndTime,
            GivenItem: this.GivenItem || "Currency:MtxPurchased",
            ItemQuantity: this.TheAmount || 1000,
            CupPlacement: this.CupPlacement || 50,
          };

          console.log(JSON.stringify(formDataObj));
          const response = await fetch(
            `${apiUrl}/admin/new/dashboard/content/cups/create`,
            {
              method: "POST",
              headers: {
                "Content-Type": "application/json",
              },
              credentials: "include",
              body: JSON.stringify(formDataObj),
            }
          );

          if (response.ok) {
            const data = await response.json();
            if(data.error == true) {
              this.ErrorMessage = data.message
            }
            else {
              console.log("why fnaf:", data);
              window.location.reload(); // ill change in the future
            }
          }
        } catch (err) {
          console.log(err);
        }
      }
    },
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
  mounted() {
    const now = new Date();

    now.setHours(now.getHours() + 1);
    this.StartTime = now.toISOString().slice(0, 16);
    console.log(this.StartTime);
    now.setHours(now.getHours() + 3); // cup lasts like 3 hours trol
    this.EndTime = now.toISOString().slice(0, 16);
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
  height: 200px; /* gotta make this dynamic in the future! */
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
