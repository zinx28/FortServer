<template>
    <h3 class="title">Account Information</h3>
    <div class="box">
        <div class="inner">
            <div class="circle"></div>
            <div class="links">
                <p href="#">{{ user?.username || "User" }}</p>
                <p href="#">{{ user?.DiscordId || "DiscordID" }}</p>
            </div>
        </div>
    </div>
</template>

<script>
export default {
    props: {
        LoginResponse: {
            type: Object,
            default: () => ({})
        }
    },
    computed: {
        user() {
            return this.LoginResponse
        }
    },
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

.box {
    width: 100%;
    height: 100%;
    padding-top: 15px;
    padding-bottom: 15px;
    background-color: #1d1c1c;
    border: 1px solid #222020;
    border-radius: 10px;
    display: flex;
    justify-content: center;
    align-items: center;
    justify-content: flex-start;
}

.inner {
    display: flex;
    align-items: center;
    gap: 15px;
    margin-left: 15px;
}

.circle {
    width: 60px;
    height: 60px;
    background-color: rgb(43, 43, 54);
    border-radius: 50%;
}

.links {
    display: flex;
    flex-direction: column;
    justify-content: center;
    gap: 5px;
}

.links p {
    margin: 0;
    text-align: left;
}

.desc {
    font-size: 15px;
    color: #636363;
    margin: 10px 0;
}

hr {
    width: 100%;
    height: 2px;
    border: none;
    background: #3a3a3a;
    margin: 14px 0;
}

.box>div h3 {
    margin-bottom: 4px;
}

button {
    padding: 8px 16px;
    border-radius: 6px;
    background: #1a1a1a;
    border: 1px solid #2d2d2d;
}
</style>
