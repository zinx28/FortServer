<template>
    <div class="CenterContent">
        <div class="TopPartUserThingf">
            <div
                style="height: 100%; width: 100px; border-top-left-radius: 10px; border-bottom-left-radius: 10px; background-color: gray;">
            </div>
            <div style="margin: 15px; width: max-content;">
                <a style="color: #9c9891;font-size: 25px; display: block; text-align: left;">
                    <strong style="color:white; ">Welcome Back,</strong> {{ user?.username || "User" }}
                </a>
                <a style="color: #9c9891; font-size: 20px; display: block; text-align: left; margin-top: 3px;">
                    This launcher is still "WIP"
                </a>
            </div>

        </div>
        <a style="color: white; font-size: 22px; font-weight: bold; text-align: left;  width: 95%;">Your Stats</a>
        <div class="GridContainer">
            <div class="GridThing">
                Win Rate
            </div>
            <div class="GridThing">
                Kills
            </div>
            <div class="GridThing">
                Vbyckjas
            </div>
            <div class="GridThing">
                Kills
            </div>
        </div>
        <a style="color: white; font-size: 20px; text-align: left;  width: 95%; ">News (soon)</a>
    </div>
</template>

<script>
export default {
    props: {
        LoginResponse: {
            type: Object,
            default: () => ({})
        },
    },
    computed: {
        user() {
            return this.LoginResponse
        }
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

.GridContainer {
    display: grid;
    grid-template-columns: repeat(4, 1fr);
    gap: 14px;
    width: fit-content;
}

.TopPartUserThingf {
    width: 95%;
    display: flex;
    background-color: #13131d;
    border-style: solid;
    border-width: 1px;
    border-color: #1e1e2d;
    height: 100px;
    border-radius: 10px;
}

.GridThing {
    width: 200px;
    background-color: #13131d;
    border-style: solid;
    border-width: 1px;
    border-color: #1e1e2d;
    height: 150px;
    border-radius: 10px;
}
</style>