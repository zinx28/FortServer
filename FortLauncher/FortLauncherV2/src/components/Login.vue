<script setup lang="ts">
</script>

<template>
    <div class="Container">

        <div class="SideImage"></div>
        <div class="LoginForm">
            <h1 style="margin-top: 50px;">FortLauncher</h1>
            <p>FortBackend is a universal backend in c#</p>

            <div class="BottomPart">
                <input type="email" class="InputBox" placeholder="Email" v-model="Email" />
                <input class="InputBox" placeholder="Password" v-model="Password">
                <div class="LoginButtonFr" @click="Login">
                    Login
                </div>
                <p>{{ ErrorMsg }}</p>
                <div class="Splitter"></div>
                <div style="margin-bottom: 30px;" class="LoginButtonFr">
                    Login with discord (disabled)
                </div>
            </div>
        </div>
    </div>
</template>

<script>
export default {
  data() {
    return {
        Email: "",
        Password: "",
        ErrorMsg: ""
    };
  },
  methods: { 
    Login() {
        console.log("NO")
        //if() client side checks
        window.ipcRenderer.invoke('fortlauncher:login~email', { email: this.Email, password: this.Password}).then(async (e) => {
            
            if(e.error == true)
                this.ErrorMsg = e.message;
          
        });
    }
  }
}

</script>

<style>
h1 {
    text-align: center;
}

.InputBox {
    width: 70%;
    padding: 10px 15px;
    font-size: 16px;
    border-radius: 10px;
    border: 2px solid #242424;
    background-color: #292929;
    color: white;
    outline: none;
    transition: all 0.3s ease;

}

.BottomPart {
    flex-direction: column;
    margin-top: auto;
    margin-bottom: 20px;
    width: 100%;
    display: flex;
    align-items: center;
    justify-content: center;
    gap: 5px;
}

.Container {
    display: flex;
    width: 100vw;
    height: 100vh;
    overflow: hidden;
}

.SideImage {
    width: 50%;
    height: 100vh;
    background: url("@/assets/LoginImage.png") no-repeat center center;
    background-size: cover;
    background-position: 10% center;
}

.LoginButtonFr {
    width: 70%;
    padding: 10px 15px;
    background-color: #2b2b2b;
    border-radius: 10px;
    text-align: center;
    display: flex;
    cursor: pointer;
    justify-content: center;
    align-items: center;
    transition: 0.3s;
  
}

.LoginButtonFr:hover {
    background-color: #333;
}

.LoginButtonFr.disabled {
    opacity: 0.5;
    cursor: not-allowed;
}


.LoginForm {
    width: 50%;
    height: 100%;
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    text-align: center;
    background-color: #1f1f1f;
    color: white;
    padding: 20px;
}

.Splitter {
    width: 75%;
    height: 3px;
    border-radius: 5px;
    background-color: #555;
    margin: 10px 0;
}
</style>