<script setup lang="ts">
import Home from './Content/Home.vue';
import Library from './Content/Library.vue';
import SideBar from './SideBar.vue';

</script>

<template>
    <SideBar @changeTab="setTab" :RizzlerTab="currentTab" :LoginResponse="getData" />
    <div v-if="currentTab === 'home'" style="margin-left: 270px;">
        <Home :LoginResponse="getData" />
    </div>
    <div v-if="currentTab === 'library'" style="margin-left: 270px;">
        <Library @buildpath="OpenBuildPopup" />
    </div>


    <div @click="OpenBuildPopup(false)" v-if="LibraryPopup" class="AddBuildPopup">
        <div @click.stop class="BuildContainer">
            <div class="TopPartIdk">
                <a style="margin-left: 20px; color: white;">Add an installition</a>
            </div>
            <div style="width: 95%; margin-top: 10px; height: 100px; border-radius: 10px; background-color: #101018;;">
                Make sure the path has "FortniteGame" and "Engine"
            </div>
            <div class="PathContainer">
                <div style=" margin-left: 20px;">
                    Choose a path
                </div>
                <div style="width: 60px;     display: flex; height: 60%; background-color: #3f3f46; margin-right: 20px; text-align: center; border-radius: 10px; padding: 2px 5px;justify-content: center;
                    align-items: center;">
                    Browse
                </div>
            </div>
            <div
                style="margin-top: auto; margin-left: auto; margin-bottom: 14px; margin-right: 20px; background-color: #18181b; border-radius: 10px; padding: 10px 15px;">
                Next ->
            </div>
        </div>
    </div>
</template>

<style scoped>
.AddBuildPopup {
    width: 100vw;
    height: 100vh;
    background: rgba(0, 0, 0, 0.7);
    position: fixed;
    display: flex;
    justify-content: center;
    align-items: center;
    top: 0;
    left: 0;
}

.PathContainer {
    width: 95%;
    margin-top: 10px;
    height: 55px;
    border-radius: 10px;
    background-color: #101018;
    display: flex;
    justify-content: space-between;
    align-items: center;
}

.TopPartIdk {
    width: 100%;
    height: 40px;
    background-color: #101018;
    top: 0;
    display: flex;
    border-top-left-radius: 10px;
    border-top-right-radius: 10px;
    align-items: center;

}

.BuildContainer {
    width: 550px;
    height: 290px;
    background-color: #13131d;
    border-radius: 10px;
    display: flex;
    flex-direction: column;
    align-items: center;
}
</style>

<script lang="ts">
export default {
    data() {
        return {
            currentTab: 'home',
            TabName: 'home',
            LibraryPopup: true
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
        OpenBuildPopup(value = true){
            this.LibraryPopup = value;
        }
    }
}
</script>