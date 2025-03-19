<script setup lang="ts">
import Home from './Content/Home.vue';
import SideBar from './SideBar.vue';

</script>

<template>
    <SideBar :LoginResponse="getData"  />
    <div style="margin-left: 270px;">
        <Home />
    </div>
</template>

<script lang="ts">
export default {
    data() {

    },
    props: {
        LoginResponse: {
            type: Object,
            default: () => ({})
        }
    },
    computed: {
        getData() {
            console.log(this.LoginResponse)
            return this.LoginResponse
        }
    }
}
</script>