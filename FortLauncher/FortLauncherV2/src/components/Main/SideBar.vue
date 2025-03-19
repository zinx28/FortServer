<template>
    <div class="SideBAr">
        <a class="title">FortLauncher</a>
        <ul class="nav-links">
            <li>
                <a class="nav-active" style="display: flex; justify-content: space-between;">
                    <span style="margin-left: 10px;">Home</span>
                </a>
            </li>
            <li>
                <a style="display: flex; justify-content: space-between;">
                    <span style="margin-left: 10px;">Library</span>
                </a>
            </li>
            <li>
                <a style="display: flex; justify-content: space-between;">
                    <span style="margin-left: 10px;">Shop</span>
                    <span
                        style=" background-color: #13131d; border-style: solid; border-width: 1px 2px; border-color: #1e1e2d; padding: 3px; font-weight:390; border-radius: 10px  ; margin-right: 10px;">coming
                        soon!</span>
                </a>
            </li>
        </ul>

        <div class="bottom">
            <div class="UserUI">
                {{ user?.username || "User" }}
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
    },
    computed: {
        user() {
            console.log(this.LoginResponse)
            return this.LoginResponse
        }
    }
}
</script>
<style scoped>
.SideBAr {
    background-color: #0f0f17;
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

.title {
    color: white;
    font-size: 30px;
    margin-top: 20px;

}

.UserUI {
    display: flex;
    align-items: center;
    justify-content: space-around;
    background-color: #13131d;
    border-style: solid;
    border-width: 1px;
    border-color: #1e1e2d;
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