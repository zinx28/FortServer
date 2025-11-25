<template>
    <div class="SideBAr">
        <a class="title">FortLauncher</a>
        <ul class="nav-links">
            <li>
                <a @click="$emit('changeTab', 'home')" style="display: flex; justify-content: space-between;"
                    :class="{ 'nav-active': tab === 'home' }">
                    <span style="margin-left: 10px;">Home</span>
                </a>
            </li>
            <li>
                <a @click="$emit('changeTab', 'library')" style="display: flex; justify-content: space-between;"
                    :class="{ 'nav-active': tab === 'library' }">
                    <span style="margin-left: 10px;">Library</span>
                </a>
            </li>
            <li>
                <a style="display: flex; justify-content: space-between;">
                    <span style="margin-left: 10px;">Shop</span>
                    <span
                        style=" background-color: #1d1c1c; border-style: solid; border-width: 1px 2px; border-color: #222020; padding: 3px; font-weight:390; border-radius: 10px  ; margin-right: 10px;">coming
                        soon!</span>
                </a>
            </li>
        </ul>

        <div class="bottom">
            <div class="UserUI">
                <p style="margin-left: 16px;">{{ user?.username || "User" }}</p>
                <a class="settings-icon" @click="$emit('changeTab', 'settings')" >
                    <svg xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink"
                        viewBox="0,0,256,256" width="30px" height="30px" fill-rule="nonzero">
                        <g fill="#ffffff" fill-rule="nonzero" stroke="none" stroke-width="1" stroke-linecap="butt"
                            stroke-linejoin="miter" stroke-miterlimit="10" stroke-dasharray="" stroke-dashoffset="0"
                            font-family="none" font-weight="none" font-size="none" text-anchor="none"
                            style="mix-blend-mode: normal">
                            <g transform="scale(10.66667,10.66667)">
                                <path
                                    d="M9.66602,2l-0.49023,2.52344c-0.82417,0.31082 -1.58099,0.74649 -2.24414,1.29102l-2.42383,-0.83594l-2.33594,4.04297l1.94141,1.6875c-0.07463,0.45823 -0.11328,0.88286 -0.11328,1.29102c0,0.40877 0.03981,0.83263 0.11328,1.29102v0.00195l-1.94141,1.6875l2.33594,4.04102l2.42188,-0.83398c0.66321,0.54482 1.42175,0.97807 2.24609,1.28906l0.49023,2.52344h4.66797l0.49024,-2.52344c0.82471,-0.31102 1.58068,-0.74599 2.24414,-1.29102l2.42383,0.83594l2.33398,-4.04102l-1.93945,-1.68945c0.07463,-0.45823 0.11328,-0.88286 0.11328,-1.29102c0,-0.40754 -0.03887,-0.83163 -0.11328,-1.28906v-0.00195l1.94141,-1.68945l-2.33594,-4.04102l-2.42188,0.83398c-0.66321,-0.54482 -1.42175,-0.97807 -2.24609,-1.28906l-0.49024,-2.52344zM11.31445,4h1.37109l0.38867,2l1.04297,0.39453c0.62866,0.23694 1.19348,0.56222 1.68359,0.96484l0.86328,0.70703l1.92188,-0.66016l0.68555,1.18555l-1.53516,1.33594l0.17578,1.09961v0.00195c0.06115,0.37494 0.08789,0.68947 0.08789,0.9707c0,0.28123 -0.02674,0.59572 -0.08789,0.9707l-0.17773,1.09961l1.53516,1.33594l-0.68555,1.1875l-1.91992,-0.66211l-0.86523,0.70898c-0.49011,0.40262 -1.05298,0.7279 -1.68164,0.96484h-0.00195l-1.04297,0.39453l-0.38867,2h-1.36914l-0.38867,-2l-1.04297,-0.39453c-0.62867,-0.23694 -1.19348,-0.56222 -1.68359,-0.96484l-0.86328,-0.70703l-1.92187,0.66016l-0.68555,-1.18555l1.53711,-1.33789l-0.17773,-1.0957v-0.00195c-0.06027,-0.37657 -0.08789,-0.69198 -0.08789,-0.97266c0,-0.28123 0.02674,-0.59572 0.08789,-0.9707l0.17773,-1.09961l-1.53711,-1.33594l0.68555,-1.1875l1.92188,0.66211l0.86328,-0.70898c0.49011,-0.40262 1.05493,-0.7279 1.68359,-0.96484l1.04297,-0.39453zM12,8c-2.19652,0 -4,1.80348 -4,4c0,2.19652 1.80348,4 4,4c2.19652,0 4,-1.80348 4,-4c0,-2.19652 -1.80348,-4 -4,-4zM12,10c1.11148,0 2,0.88852 2,2c0,1.11148 -0.88852,2 -2,2c-1.11148,0 -2,-0.88852 -2,-2c0,-1.11148 0.88852,-2 2,-2z">
                                </path>
                            </g>
                        </g>
                    </svg>
                </a>
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
        },
        RizzlerTab: {
            type: String,
            default: () => ''
        }
    },
    computed: {
        user() {
            console.log(this.LoginResponse)
            return this.LoginResponse
        },
        tab() {
            return this.RizzlerTab
        }
    }
}
</script>
<style scoped>
.SideBAr {
    background-color: #141414;
    width: 270px;
    height: 100vh;
    display: flex;
    flex-direction: column;
    align-items: center;
    position: fixed;
    left: 0;
    top: 0;
}

.bottom {
    margin-top: auto;
    margin-bottom: 10px;
    justify-content: center;
    align-items: center;
}

.settings-icon {
  margin-left: auto;
  margin-right: 12px;
  display: flex;
  align-items: center;
}

.title {
    color: white;
    font-size: 30px;
    margin-top: 20px;

}

.UserUI {
    display: flex;
    align-items: center;
    justify-content: space-around;
    background-color: #1d1c1c;
    border-style: solid;
    border-width: 1px;
    border-color: #222020;
    width: 234px;
    height: 50px;
    border-radius: 10px;
    gap: 10px;
}

.nav-links {
    display: flex;
    list-style: none;
    flex-direction: column;
    justify-content: center;
    align-items: center;
    width: 100%;
    margin-top: 30px;
}

.nav-links li {
    width: 100%;
    margin-bottom: 10px;
}

.nav-links a {
    display: flex;
    justify-content: flex-start;
    align-items: center;
    text-align: left;
    text-decoration: none;
    color: #ffffff;
    font-weight: bolder;
    font-size: 14px;
    transition: color 0.3s, background-color 0.3s;
    width: 240px;
    height: 40px;
    border-radius: 10px;
    gap: 10px;
    font-weight: lighter;
}

.nav-links a:hover {
    color: white;
    transition: color 0.3s, background-color 0.3s;
    background: rgba(102, 102, 102, 0.1)
}

.nav-links a.nav-active {
    background: rgba(109, 109, 109, 0.1)
}
</style>