import { createApp } from "vue";
import store from "./store";
import "./style.css";
import App from "./App.vue";
import NotFound from "./components/NotFound.vue";
import Login from "./components/Login.vue";
import { createRouter, createWebHistory } from "vue-router";
import VueCookies from "vue-cookies";
import { createPinia } from "pinia";

const router = createRouter({
  history: createWebHistory(),
  routes: [
    {
      path: "/login",
      name: "Login",
      component: () => Login,
      beforeEnter: async (_, __, next) => {
        if (!store().isAuthenticated) {
          store()
            .checkAuth()
            .then(() => {
              if (store().isAuthenticated) router.push({ name: "Dashboard" });
            });
        } else {
          router.push({ name: "Dashboard" });
        }

        // console.log("frfr " + VueCookies.VueCookies.keys())
        //console.log(VueCookies.VueCookies.get("AuthToken"))
        next();
      },
    },
    {
      path: "/dashboard",
      name: "Dashboard",
      component: () => import("./components/Dashboard.vue"),
      meta: { requiresAuth: true },
    },
    {
      path: "/dashboard/content",
      name: "DashboardContent",
      redirect: '/dashboard/content/news',
    },
    {
      path: "/dashboard/content/:id",
      name: "DashboardContentID",
      component: () => import("./components/DashboardContent.vue"),
      meta: { requiresAuth: true },
    },
    {
      path: "/dashboard/admin",
      name: "DashboardAdmin",
      component: () => import("./components/DashboardAdmin.vue"),
      meta: { requiresAuth: true },
    },

    {
      path: "/:pathMatch(.*)*", // nothing special just 404 page
      name: "NotFound",
      component: NotFound,
    },
  ],
});

router.beforeEach(async (to, from, next) => {
  if (to.meta.requiresAuth) {
    if (!store().isAuthenticated) {
        store()
        .checkAuth()
        .then(() => {
          if (store().isAuthenticated) {
             if(from.fullPath.includes("login")){
                router.push({ name: "Dashboard" });
             }
          }
          else router.push({ name: "Login" });
        });
    }
  }

  next();
});

console.log(import.meta.env.VITE_PORT);

var app = createApp(App);
app.use(VueCookies, { expires: "7d" });
app.use(createPinia());
//app.use(store);
app.use(router).mount("#app");
