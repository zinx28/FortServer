<template>
  <h2>RESTART BACKEND AFTER CHANGE!</h2>
  <div class="table-container">
    <button disabled>Restart Backend (DISABLED)</button>
    <br />
    <button v-if="BackButton" @click="GoBACK">BACK</button>
    <button v-if="BackButton" @click="SaveContent">Save</button>
    <table>
      <thead>
        <tr class="header-row">
          <th>Section</th>
          <th>Body</th>
          <th>Action</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="(editthing, index) in sections" :key="index" class="row">
          <td>{{ editthing.Key || editthing.Title }}</td>
          <td>{{ editthing.Description || editthing.Type }}</td>
          <td>
            <button
              v-if="editthing.Description"
              type="text"
              @click="EditSection(index)"
            >
              Edit
            </button>
            <input
              :type="
                editthing.Private
                  ? 'password'
                  : editthing.Type == 'int'
                  ? 'number'
                  : 'text'
              "
              v-if="!editthing.Description"
              :value="editthing.Value"
              @input="markAsModified(index, $event.target.value)"
            />
          </td>
        </tr>
      </tbody>
    </table>
  </div>
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
.table-container {
  overflow: hidden;
}

table {
  width: 100%;
  border-collapse: separate;
  border-spacing: 0;
}

thead {
  background-color: #424242;
}

.header-row th:first-child {
  border-top-left-radius: 10px;
  border-bottom-left-radius: 10px;
}

.header-row th:last-child {
  border-top-right-radius: 10px;
  border-bottom-right-radius: 10px;
}

.row {
  border-bottom: 3px double #ddd;
}

td,
th {
  padding: 12px;
  text-align: left;
}
</style>

<script>
export default {
  data() {
    return {
      sections: [],
      sectionsData: [],
      BackButton: false,
      modifiedSections: new Map(),
      IDOfSection: 0,
    };
  },
  methods: {
    GoBACK() {
      this.modifiedSections = new Map(); // imagine if ninja
      this.sections = this.sectionsData;
      this.BackButton = false;
    },
    async EditSection(hihi, isdiff) {
      const apiUrl = import.meta.env.VITE_API_URL;
      const apiResponse = await fetch(
        `${apiUrl}/admin/new/dashboard/content/ConfigData/${hihi}`,
        {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
          },
          credentials: "include",
        }
      );
      var responsefr = await apiResponse.json();
      if (responsefr) {
        console.log(JSON.stringify(responsefr.Data));
        this.sections = responsefr.Data;
      }
      //console.log("E " + this.sectionsData[hihi].Data);
      this.IDOfSection = hihi;
      //this.sections = this.sectionsData[hihi].Data;
      this.BackButton = true;
    },
    markAsModified(index, value) {
      console.log(this.sections[index]);
      if (typeof this.sections?.[index]?.Type === "bool") {
        value =
          value === "true" ? "true" : value === "false" ? "false" : "false";
      } else if (typeof this.sections?.[index]?.Value === "number") {
        value = parseInt(value, 10).toString() || "0";
      }

      this.modifiedSections.set(index, value);
    },
    async SaveContent() {
      if (this.modifiedSections.size == 0) return;

      const modifiedData = Array.from(this.modifiedSections.entries()).map(
        ([index, value]) => {
          //console.log(index, value);
          return { index, value };
        }
      );
      console.log(modifiedData);

      const apiUrl = import.meta.env.VITE_API_URL;
      try {
        const response = await fetch(
          `${apiUrl}/dashboard/v2/content/update/config/${this.IDOfSection}/69`, // real ones
          {
            method: "POST",
            headers: {
              "Content-Type": "application/json",
            },
            body: JSON.stringify(modifiedData),
            credentials: "include",
          }
        );

        const JsonParsed = await response.json();

        if (JsonParsed) {
          if (JsonParsed.error == false) {
            console.log(JsonParsed);
          }
        }
      } catch (err) {
        console.log(err);
      }
    },
  },
  async mounted() {
    const apiUrl = import.meta.env.VITE_API_URL;
    const apiResponse = await fetch(
      `${apiUrl}/admin/new/dashboard/content/ConfigData`,
      {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        credentials: "include",
      }
    );

    var ApiRe = await apiResponse.json();
    if (ApiRe) {
      this.sections = ApiRe;
      this.sectionsData = ApiRe;
    }
    console.log(ApiRe);
  },
};
</script>
