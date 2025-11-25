import { createApp } from "vue";
import "./style.css";
import App from "./App.vue";
import router from "./router/router";
import VueToast from "vue-toast-notification";
import "vue-toast-notification/dist/theme-sugar.css";

createApp(App).use(VueToast).use(router).mount("#app");
