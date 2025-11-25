<script setup lang="ts">
import Home from './Content/Home.vue';
import SideBar from './SideBar.vue';
import Library from './Content/Library.vue';
import Settings from './Content/Settings.vue';

</script>

<template>
    <SideBar @changeTab="setTab" :RizzlerTab="currentTab" :LoginResponse="getData" />
    <div v-if="currentTab === 'home'" style="margin-left: 270px;">
        <Home :LoginResponse="getData" />
    </div>
    <div v-if="currentTab === 'library'" style="margin-left: 270px;">
        <Library ref="libraryTab" @buildpath="OpenBuildPopup" />
    </div>
    <div v-if="currentTab === 'settings'" style="margin-left: 270px;">
        <Settings ref="libraryTab"  />
    </div>

    <div @click="OpenBuildPopup(false)" v-if="LibraryPopup" class="AddBuildPopup">
        <div @click.stop class="BuildContainer">
            <div class="TopPartIdk">
                <a style="margin-left: 20px; color: white;">Add an installition</a>
            </div>
            <div v-if="!NextStep"
                style="width: 95%; margin-top: 10px; height: 100px; border-radius: 10px; background-color: #101018;">
                Make sure the path has "FortniteGame" and "Engine"
            </div>
            <div v-if="!NextStep" class="PathContainer">
                <div style=" margin-left: 20px;">
                    {{ GamePath }}
                </div>
                <div @click="OpenFileExplorer" style="width: 60px;     display: flex; height: 60%; background-color: #3f3f46; margin-right: 20px; text-align: center; border-radius: 10px; padding: 2px 5px;justify-content: center;
                    align-items: center;">
                    Browse
                </div>
            </div>

            <div v-if="NextStep"
                style="height: 100%; width: 100%; display: flex; flex-direction: column; align-items: center; justify-content: center;">
                <a style="margin-top: 20px;  display: block; color: white;">Import Installation</a>
                <div
                    style="width: 95%; margin-top: 10px; height: 100px; border-radius: 10px; background-color: #101018;;">
                    Version: {{ FortniteVersion }}
                </div>
            </div>

            <div @click="BuildNextButton"
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
            LibraryPopup: false,
            GamePath: "Choose a path",
            GamePath2: "",
            NextStep: false,
            ErrorIfSoNotSigma: false,
            FortniteVersion: ""
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
        OpenBuildPopup(value = true) {
            this.LibraryPopup = value;
            this.GamePath = 'Choose a path';
            this.GamePath2 = '';
            this.NextStep = false; // reset duh!
        },
        OpenFileExplorer() {
            window.ipcRenderer.invoke("fortlauncher:openfile").then((filepath) => {
                if (filepath) {
                    if (filepath.startsWith('Error~~')) {
                        console.log("darn it man!");
                    }
                    else {
                        console.log("hi");
                        this.GamePath = filepath;
                        this.GamePath2 = filepath;
                    }
                    console.log(filepath + "sigmais");
                }
            })
        },
        // i didnt know what to call this
        async BuildNextButton() {
            if (this.GamePath2 && this.GamePath2 != '' && !this.NextStep) {
                this.NextStep = true;
                const AddPath = await window.ipcRenderer.invoke('fortlauncher:addpath', { PathValue: this.GamePath2 })
                if (!AddPath || AddPath.error) {
                    console.log("cookde");

                }
                else {
                    console.log(AddPath);

                    this.FortniteVersion = AddPath.data.VersionID
                }

            } else if (this.NextStep) {
                const AddPathV2 = await window.ipcRenderer.invoke('fortlauncher:addpathV2')

                if (AddPathV2 && !AddPathV2.startsWith('Error')) {
                    this.OpenBuildPopup(false);

                    (this.$refs.libraryTab as any).loadBuilds(true);

                }

            }
        }
    }
}
</script>