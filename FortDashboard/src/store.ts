// store

import { defineStore } from "pinia";

export const useAuthStore = defineStore('auth', {
  state: () => ({
    isAuthenticated: false,
    setup: true, // this will change to smth else in the future
    displayName: ""
  }),
 // mutations: {
   // setAuthenticated(state: State, status: any) {
     //   console.log("YES + " + status.displayName);
    //  state.isAuthenticated = true;
    //  state.displayName = status.displayName
    //},
  //},
  actions: {
    async checkAuth() {
      const apiUrl = import.meta.env.VITE_API_URL;
      try {
        const response = await fetch(`${apiUrl}/admin/new/login/check`, {
          method: "POST",
          headers: {
            "Content-Type": "application/x-www-form-urlencoded",
          },
          credentials: "include",
        });

        const JsonParsed = await response.json();
        console.log(JsonParsed);
        if (JsonParsed && !JsonParsed.error) {
          this.displayName = JsonParsed.displayName;
          this.isAuthenticated = true;
          this.setup = JsonParsed.setup
        } else {
         // commit("setAuthenticated", false);
        }
      } catch (error) {
        console.error("Error checking authentication:", error);
        //commit("setAuthenticated", false);
      }
    },
  },
});

export default useAuthStore;