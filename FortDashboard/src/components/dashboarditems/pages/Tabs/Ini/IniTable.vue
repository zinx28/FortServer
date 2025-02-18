<template>
  <div class="table-container">
    <table>
      <thead>
        <tr class="header-row">
          <th>Section</th>

          <th>Action</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="(editthing, index) in sections" :key="index" class="row">
          <td>{{ editthing.Name }}</td>
          <td>
            <input
              type="text"
              :value="editthing.Value"
              @input="markAsModified(index, $event.target.value)"
            />
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<script>
export default {
  props: {
    sections: Array,
  },
  methods: {
    markAsModified(index, value) {
      this.$emit("update-section", index, value);
    },
  },
};
</script>

<style scoped>
input {
  accent-color: #4caf50;
  border: 0px;
  border-radius: 8px;
  padding: 0.5rem;
  background-color: #181717;
  transition: border 0.3s ease, background-color 0.3s ease;
}

input[type="checkbox"]:disabled,
input[type="number"]:disabled {
  background-color: #161616;
}

input[type="number"]:focus {
  outline: none;
  border: 2px solid #007bff;
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
