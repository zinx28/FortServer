<template>
  <div class="sidebar">
    <div class="SideBarTopOuter">
      <span class="SideBarTitle" style="">FortBackend</span>
    </div>
    <nav>
      <ul>
        <li class="nav-item">
          <a
            @click="$emit('changeTab', 'home')"
            class="NavButton"
            :class="{ active: NavResponse.currentTab === 'home' }"
          >
            <svg
              xmlns="http://www.w3.org/2000/svg"
              width="16"
              height="16"
              fill="currentColor"
              class="bi bi-house-fill"
              viewBox="0 0 16 16"
            >
              <path
                d="M8.707 1.5a1 1 0 0 0-1.414 0L.646 8.146a.5.5 0 0 0 .708.708L8 2.207l6.646 6.647a.5.5 0 0 0 .708-.708L13 5.793V2.5a.5.5 0 0 0-.5-.5h-1a.5.5 0 0 0-.5.5v1.293z"
              />
              <path
                d="m8 3.293 6 6V13.5a1.5 1.5 0 0 1-1.5 1.5h-9A1.5 1.5 0 0 1 2 13.5V9.293z"
              />
            </svg>
            Dashboard
          </a>
        </li>
        <li class="nav-item">
          <a
            @click="$emit('changeTab', 'content')"
            class="NavButton"
            :class="{ active: NavResponse.currentTab === 'content' }"
          >
            <svg
              xmlns="http://www.w3.org/2000/svg"
              width="16"
              height="16"
              fill="currentColor"
              class="bi bi-window-stack"
              viewBox="0 0 16 16"
            >
              <path
                d="M4.5 6a.5.5 0 1 0 0-1 .5.5 0 0 0 0 1M6 6a.5.5 0 1 0 0-1 .5.5 0 0 0 0 1m2-.5a.5.5 0 1 1-1 0 .5.5 0 0 1 1 0"
              ></path>
              <path
                d="M12 1a2 2 0 0 1 2 2 2 2 0 0 1 2 2v8a2 2 0 0 1-2 2H4a2 2 0 0 1-2-2 2 2 0 0 1-2-2V3a2 2 0 0 1 2-2zM2 12V5a2 2 0 0 1 2-2h9a1 1 0 0 0-1-1H2a1 1 0 0 0-1 1v8a1 1 0 0 0 1 1m1-4v5a1 1 0 0 0 1 1h10a1 1 0 0 0 1-1V8zm12-1V5a1 1 0 0 0-1-1H4a1 1 0 0 0-1 1v2z"
              ></path>
            </svg>
            Content Management
          </a>
        </li>
        <li class="nav-item">
          <a
            @click="$emit('changeTab', 'admin')"
            class="NavButton"
            :class="{ active: NavResponse.currentTab === 'admin' }"
          >
            <svg
              xmlns="http://www.w3.org/2000/svg"
              width="16"
              height="16"
              fill="currentColor"
              class="bi bi-layout-text-window"
              viewBox="0 0 16 16"
            >
              <path
                d="M3 6.5a.5.5 0 0 1 .5-.5h5a.5.5 0 0 1 0 1h-5a.5.5 0 0 1-.5-.5m0 3a.5.5 0 0 1 .5-.5h5a.5.5 0 0 1 0 1h-5a.5.5 0 0 1-.5-.5m.5 2.5a.5.5 0 0 0 0 1h5a.5.5 0 0 0 0-1z"
              ></path>
              <path
                d="M2 0a2 2 0 0 0-2 2v12a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V2a2 2 0 0 0-2-2zm12 1a1 1 0 0 1 1 1v1H1V2a1 1 0 0 1 1-1zm1 3v10a1 1 0 0 1-1 1h-2V4zm-4 0v11H2a1 1 0 0 1-1-1V4z"
              ></path>
            </svg>
            Admin Panel
          </a>
        </li>
      </ul>
    </nav>
    <div class="BottomDiV">
      <hr />
      <div class="BottomContainer">
        <div class="UserInfo">
          <div class="UserImageDiv"></div>
          <span>{{ NavResponse.displayName }}</span>
        </div>
        <div class="RightContainerFR" @click="toggleDropdown">
          <svg
            class="DownArrow"
            xmlns="http://www.w3.org/2000/svg"
            viewBox="0 0 24 24"
            width="24"
            height="24"
          >
            <path fill="currentColor" d="M7 10l5 5 5-5H7z"></path>
          </svg>
        </div>
      </div>
      <div class="DropdownMenu" v-if="isDropdownOpen">
        <div class="DropdownItem">More Soon!</div>
        <hr />
        <div class="DropdownItem" @click="SignOut">Sign Out</div>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  props: {
    NavResponse: {
      type: Object,
      default: () => ({}),
    },
  },
  data() {
    return {
      isDropdownOpen: false,
    };
  },
  methods: {
    toggleDropdown(event) {
      event.stopPropagation();
      this.isDropdownOpen = !this.isDropdownOpen;
      if (this.isDropdownOpen) {
        // fancy way of only adding it once
        setTimeout(() => {
          window.addEventListener("click", this.handleClickOutside);
        });
      }
    },
    async SignOut() {
      const apiUrl = import.meta.env.VITE_API_URL;
      const response = await fetch(`${apiUrl}/admin/new/logout`, {
        method: "POST",
        headers: {
          "Content-Type": "application/x-www-form-urlencoded",
        },
        credentials: "include",
      });

      const JsonParsed = await response.json();

      if (JsonParsed) {
        window.location.reload();
      }
    },
    handleClickOutside(event) {
      const appRoot = document.getElementById("app");
      if (
        appRoot.contains(event.target) &&
        !event.target.classList.contains("DropdownMenu")
      ) {
        window.removeEventListener("click", this.handleClickOutside);
        this.isDropdownOpen = false;
      }
    },
  },
};
</script>
<style>
body {
  margin: 0;
}
.SideBarTitle {
  font-size: 20px;
  font-family: "Raleway", sans-serif;
}
.SideBarTopOuter {
  height: 90px;
  align-items: center;
  align-content: center;
}
.sidebar {
  background-color: #1d1f20;
  color: white;
  width: 250px;
  min-width: 250px; /* flex dir shrinks it lol on small screens */
  height: 100vh;
  display: flex;
  position: fixed;
  flex-direction: column;
}

.UserInfo {
  display: flex;
  align-items: center;
  margin-left: 15px;
  gap: 15px;
}

.NavButton {
  display: flex;
  width: 90%;
  padding: 0.5rem 1rem;
  border-radius: 5px;
  text-decoration: none;
  color: white; /* makes it only show the color on hover bc ykykyk!! */
  text-align: left;
  font-size: 13px;
  gap: 8px;
  align-items: center;
  cursor: pointer;
  transition: background-color 0.3s ease, transform 0.2s ease;
}
.NavButton.active {
  background-color: #313335;
}
.NavButton:hover {
  background-color: #313335;
}
nav ul {
  list-style: none;
  padding: 0;
  margin: 0;
}
.nav-item {
  margin-bottom: 1rem;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-wrap: wrap;
}
.sidebar nav {
  display: flex;
  flex-direction: column;
  padding: 1rem;
}
.BottomDiV {
  display: flex;
  flex-direction: column;
  margin-top: auto;
  margin-bottom: 10px;
  gap: 15px;
  text-align: center;
  justify-content: center;
  padding: 0 10px;
}
.BottomContainer {
  display: flex;
  justify-content: center;
  align-items: center;
  justify-content: space-between;
  background-color: #313335;
  width: 100%;
  height: 50px;
  border-radius: 10px;
}
.UserImageDiv {
  width: 30px;
  height: 30px;
  background-color: rgb(129, 23, 228);
  border-radius: 15px;
}
.RightContainerFR {
  margin-right: 15px;
}

.DropdownMenu {
  position: absolute;
  bottom: 40px;
  left: 85px;
  background-color: #313335;
  border: 1px solid #313335;
  border-radius: 4px;
  width: 150px;
  box-shadow: 0px 2px 5px rgba(0, 0, 0, 0.1);
  display: flex;
  flex-direction: column;
  transform: translate3d(0px, -26.1818px, 0px);
  z-index: 1000;
}

.DropdownItem {
  padding: 8px 10px;
  color: white;
  cursor: pointer;
  font-size: 1rem;
}

.DropdownItem:hover {
  background-color: #2b2c2e;
  color: #535bf2;
}
</style>
