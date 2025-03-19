<script setup lang="ts">
import Home from './Content/Home.vue';
import Library from './Content/Library.vue';
import SideBar from './SideBar.vue';

</script>

<template>
    <SideBar @changeTab="setTab" :RizzlerTab="currentTab" :LoginResponse="getData" />
    <div v-if="currentTab === 'home'"  style="margin-left: 270px;">
        <Home :LoginResponse="getData" />
    </div>
    <div v-if="currentTab === 'library'"  style="margin-left: 270px;">
        <Library />
    </div>
</template>

<script lang="ts">
export default {
    data() {
        return {
            currentTab: 'home',
            TabName: 'home',
        }
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
    },
    methods: {
        setTab(tab: string) {
            if (tab != this.TabName) {
                this.TabName = tab
                this.currentTab = tab
            }
        },
    }
}
</script>