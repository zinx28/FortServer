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
          <td>{{ editthing.Title || editthing }}</td>
          <td><button @click="$emit('edit', /*editthing.Title ||*/ index)">Edit</button></td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<script>
export default {
  props: {
    sections: Array
  }
};
</script>

<style scoped>
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

td, th {
  padding: 12px;
  text-align: left;
}




</style>
