<template>
    <div class="CenterContent">
        <div class="TopPartUserThingf">
            <div :style="{
                height: '100%',
                width: '100px',
                borderTopLeftRadius: '10px',
                borderBottomLeftRadius: '10px',
                backgroundImage: user?.character
                    ? `url('https://fortnite-api.com/images/cosmetics/br/${user.character}/icon.png')`
                    : 'none',
                backgroundSize: 'cover',
                backgroundPosition: 'center'
            }">
            </div>
            <div style="margin: 15px; width: max-content;">
                <a style="color: #9c9891;font-size: 25px; display: block; text-align: left;">
                    <strong style="color:white;">Welcome Back,</strong> {{ user?.username || "User" }}
                </a>
                <a style="color: #9c9891; font-size: 20px; display: block; text-align: left; margin-top: 3px;">
                    This launcher is still "WIP"
                </a>
            </div>

        </div>
        <a style="color: white; font-size: 22px; font-weight: bold; text-align: left;  width: 95%;">Your Stats</a>
        <div class="GridContainer">
            <div class="GridThing">
                <p>Matches Played</p>
                <p>{{ user?.stats?. MatchesPlayed }}</p>
            </div>
            <div class="GridThing">
                <p>Kills</p>
                <p>{{ user?.stats?.Kills }}</p>
            </div>
            <div class="GridThing">
                <p>Vbucks</p>
                <p>{{ user?.vbucks }}</p>
            </div>
            <div class="GridThing">
                <p>Wins</p>
                <p>{{ user?.stats?.Wins }}</p>
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
        }
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
    background-color: #1d1c1c;
    border-style: solid;
    border-width: 1px;
    border-color: #222020;
    height: 100px;
    border-radius: 10px;
}

.GridThing {
    width: 200px;
    background-color: #1d1c1c;
    border-style: solid;
    border-width: 1px;
    border-color: #222020;
    height: 100px;
    border-radius: 10px;
}
</style>