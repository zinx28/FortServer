<template>
    <div class="tabs">
      <button
        v-for="(file, index) in files"
        :key="index"
        :class="{ active: index === activeTab }"
        @click="selectTab(index)"
      >
        {{ file.FileName }}
      </button>
    </div>
    <div class="thingundertab">
      <button disabled style="margin-left: auto; margin-right: 0px;">New</button>
    </div>
  </template>
  
  <script>
  export default {
    props: {
      files: Array,
      activeTab: Number
    },
    methods: {
      selectTab(index) {
        this.$emit("tab-selected", index);
      }
    }
  };
  </script>
  
  <style scoped>
  .tabs {
    display: flex;
    gap: 10px;
    margin-bottom: 10px;
  }
  .thingundertab {
    display: flex;
    margin-bottom: 10px;
  }
  button {
    font-size: 14px;
  }
  .active {
    background: #ffffff;
    color: rgb(0, 0, 0);
  }
  </style>
  