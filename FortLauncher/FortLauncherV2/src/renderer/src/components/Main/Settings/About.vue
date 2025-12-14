<template>
    <h3 class="title">About FortLauncher</h3>
    <div class="box">
        <div style="width: 90%;">
            <h4>FortLauncher</h4>
            <hr>
            <h5 class="desc">FortBackend is a Universal Fortnite Private Server written in C#.</h5>
            <hr>
            <div class="actions">
                <button @click="goGitHub()">View on GitHub</button>
            </div>
        </div>

    </div>
</template>

<script>
export default {
    methods: {
        goGitHub() {
            window.ipcRenderer.invoke('open-link', "https://github.com/zinx28/FortServer")
        }
    }
}
</script>

<style scoped>
.title {
    width: 100%;
    text-align: left;
}

.box>div {
    display: flex;
    flex-direction: column;
    align-items: flex-start;
}

.desc {
    font-size: 15px;
    color: #808080;
    margin: 10px 0;
}

hr {
    width: 100%;
    height: 2px;
    border: none;
    background: #3a3a3a;
    margin: 14px 0;
}

.box>div h4 {
    margin-bottom: 4px;
}

button {
    padding: 8px 16px;
    border-radius: 6px;
    background: #1a1a1a;
    border: 1px solid #2d2d2d;
}

.box {
    width: 100%;
    height: 100%;
    padding-bottom: 15px;
    background-color: #1d1c1c;
    border-style: solid;
    border-width: 1px;
    border-color: #222020;
    border-radius: 10px;
    margin-left: 0;
    display: flex;
    align-items: center;
    justify-content: center;
}
</style>