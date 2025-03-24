<template>
    <div class="CenterContent">
        <a style="color: white; font-size: 22px; font-weight: bold; text-align: left;  width: 95%;">Library</a>
        <div class="GridContainer">
            <div  v-for="(note, index) in builds" class="GridThing">
                <div class="BuildImageHOlder">
                    <div class="launchButton">
                        Launch
                    </div>
                </div>
                <div class="SomeDivINthebuild">
                    <a class="BuildTitle"> Fortnite </a>
                    <a class="BuildBody">{{ versionCache[note.VersionID] || 'Loading...' }}</a>
                </div>
            </div>
            <div class="GridThing">
                <div @click="$emit('buildpath')" class="AddBuildB">
                    <div style="width: 50px; height: 50px; background-color: gray; border-radius: 40px;">

                    </div>
                    <a style="color:white; margin-top: 10px;">Import</a>
                    <a style="color: #9c9891; font-size: 12px; margin-top: 2px;">Add existing installation</a>
                </div>
            </div>
        </div>
    </div>

</template>

<script>
export default {
    data() {
        return {
            builds: [],
            versionCache: {},
            dataLoaded: false
        }
    },
    methods: {
        async loadBuilds(shouldturnfalse) {
            if (shouldturnfalse) this.dataLoaded = false
            console.log(this.dataLoaded)
            if (this.dataLoaded) return
            try {
                const response = await window.ipcRenderer.invoke('fortlauncher:get-builds')
                this.builds = response
                console.log('NIG' + this.builds)

                for (const note of this.builds) {
                    if (note.VersionID && !this.versionCache[note.VersionID]) {
                        this.versionCache[note.VersionID] = await window.ipcRenderer.invoke(
                            'fortlauncher:getBuildVersion',
                            note.VersionID
                        )
                    }
                }
                this.dataLoaded = true
            } catch (error) {
                console.error('Failed to load builds:', error)
            }
        },
    },
    mounted() {
        this.loadBuilds()
    }
}
</script>

<style scoped>
.CenterContent {
    margin-top: 15px;
    display: flex;
    flex-direction: column;
    width: 100%;
    align-items: center;
    gap: 15px;
}

.SomeDivINthebuild {
    display: flex;
    flex-direction: column;
    width: 100%;
    align-items: flex-start;
    margin-top: 5px;
}

.BuildImageHOlder .launchButton {
    opacity: 0;
    visibility: hidden;
    transition: opacity 0.3s ease;
}


.BuildImageHOlder:hover {
    background-color: #5f5f5ffb;
}


.BuildImageHOlder:hover .launchButton {
    opacity: 1;
    visibility: visible;
}

.launchButton {
    background-color: #1c1c1e;
    color: white;
    border: none;
    padding: 8px 0px;
    width: 90%;
    border-radius: 5px;
    cursor: pointer;
    font-size: 16px;
    margin-top: auto;
    margin-bottom: 10px;
}

.GridContainer {
    display: grid;
    grid-template-columns: repeat(auto-fit, 170px);
    gap: 15px;
    width: 95%;
    justify-content: start;

}

.BuildTitle {
    color: white;
    font-size: 18px;
    font-weight: bold;
    margin-top: 0px;
    margin-left: 10px;
}

.BuildBody {
    color: #9c9891;
    font-size: 14px;
    font-weight: normal;
    margin-top: 0px;
    margin-left: 10px;
}

.BuildImageHOlder {
    width: 170px;
    background-color: #a5a5a5;
    border-style: solid;
    border-width: 1px;
    border-color: #1e1e2d;
    height: 225px;
    border-radius: 10px;
    display: flex;
    justify-content: center;
    align-items: center;
    position: relative;
    overflow: hidden;
    transition: all 0.3s ease;
}


.AddBuildB {
    width: 170px;
    background-color: transparent;
    border-style: dotted;
    border-width: 2px;
    border-color: #a0a0b8;
    height: 225px;
    border-radius: 10px;
    display: flex;
    justify-content: center;
    align-items: center;
    position: relative;
    overflow: hidden;
    transition: all 0.3s ease;
    flex-direction: column;
}

.GridThing {
    display: flex;
    flex-direction: column;
    width: 170px;
    height: 295px;
    justify-content: flex-start;
    border-radius: 10px;
    align-items: flex-start;
}
</style>