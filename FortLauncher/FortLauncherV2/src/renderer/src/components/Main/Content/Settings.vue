<template>
    <div class="CenterContent">
        <a style="color: white; font-size: 22px; font-weight: bold; text-align: left;  width: 95%;">Settings, unfinished</a>
        
        <button class="open-appdata" @click="openAppData">Open AppData</button>
        <button class="open-appdata" @click="LogoutLauncher">Logout</button>
    </div>

</template>

<script>
export default {
  methods: {
    async openAppData() {
      try {
        await window.ipcRenderer.invoke("fortlauncher:open-appdata");
      } catch (err) {
        console.error("Failed to open AppData:", err);
      }
    },
    async LogoutLauncher() {
      try {
        await window.ipcRenderer.invoke("fortlauncher:logout");
      } catch (err) {
        console.error("Failed to logout:", err);
      }
    }
  }
}
</script>

<style scoped>
.open-appdata {
  margin-top: 8px;
}
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

.launchButton.running {
    background-color: #27ae60;
}

.launchButton:disabled {
    background-color: #bdc3c7;
    cursor: not-allowed;
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
    width: 90%;
    height: 170px;
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
    height: 290x;
    justify-content: flex-start;
    border-radius: 10px;
    align-items: flex-start;
}
</style>