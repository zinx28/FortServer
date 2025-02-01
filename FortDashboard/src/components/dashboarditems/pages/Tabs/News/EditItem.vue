<template>
  <div
    style="display: flex; width: 100%; flex-direction: column; margin-top: 25px"
  >
    <button @click="$emit('back')" style="width: 130px">Back</button>
    <br />
    <h2>{{ DATA.Type }}</h2>
    <form @submit.prevent="saveData" class="FormClass">
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
        <textarea
          type="text"
          name="body"
          v-model="body"
          class="FormInput"
          id="floatingInput"
          placeholder=" "
          required
        ></textarea>
        <label for="floatingInput" class="FormLabel">Body</label>
      </div>

      <h5 :style="{ display: imageDisabled ? 'none' : 'block' }">
        Dashboard doesnt support images attachments but to make up we allow you
        to change the image url!
      </h5>

      <div
        class="FormGroup"
        :style="{ display: imageDisabled ? 'none' : 'block' }"
      >
        <input
          type="text"
          name="image"
          v-model="image"
          class="FormInput"
          id="floatingInput"
          :disabled="imageDisabled"
          placeholder=" "
          required
        />
        <label for="floatingInput" class="FormLabel">Image Url</label>
      </div>
      <p>{{ ErrorMessage }}</p>
      <button>Save</button>
    </form>
  </div>
</template>

<script>
export default {
  data() {
    return {
      title: "",
      body: "",
      image: "",
      imageDisabled: false,
      ErrorMessage: "",
    };
  },
  props: {
    DATA: Object,
    IDOfSection: Number,
    EditPart: Number,
  },
  methods: {
    async saveData() {
      const apiUrl = import.meta.env.VITE_API_URL;
      console.log({
        title: this.title,
        body: this.body,
        ...(this.image ? { image: this.image } : {}),
      });
      const response = await fetch(
        `${apiUrl}/dashboard/v2/content/update/news/${this.IDOfSection}/${this.EditPart}`,
        {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
          },
          body: JSON.stringify({
            title: this.title,
            body: this.body,
            ...(this.image ? { image: this.image } : {}),
          }),
          credentials: "include",
        }
      );

      const JsonParsed = await response.json();

      console.log(JsonParsed);

      this.ErrorMessage = JsonParsed.message;
    },
  },
  mounted() {
    console.log("A " + JSON.stringify(this.DATA));
    if (this.DATA.Data) {
      if (this.DATA.Data.title && this.DATA.Data.title.en) {
        this.title = this.DATA.Data.title.en;
      } else {
        if (this.DATA.Data.playlist_name)
          this.title = this.DATA.Data.playlist_name;
      }
      if (this.DATA.Data.body && this.DATA.Data.body.en) {
        this.body = this.DATA.Data.body.en;
      } else {
        if (this.DATA.Data.description)
          this.body = this.DATA.Data.description.en;
      }
      if (this.DATA.Data.image) this.image = this.DATA.Data.image;
      else this.imageDisabled = true;
    }
  },
};
</script>

<style scoped>
.FormClass {
  display: flex;
  text-align: center;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  width: 500px;
  margin-left: 20px;
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
</style>
