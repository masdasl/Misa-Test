import { createRouter, createWebHistory } from "vue-router";
import DeskDetail from "../pages/Desk/desk-detail/DeskDetail.vue";
import HomeDetail from "../pages/Home/home-detail/HomeDetail.vue";
import PageNotFound from "../pages/NotFound/PageNotFound.vue";
import { customerChildRoutes } from "../pages/Customer/router";

const routes = [
  {
    path: "/",
    name: "home",
    component: HomeDetail,
  },
  {
    path: "/desk",
    name: "desk",
    component: DeskDetail,
  },
  {
    path: "/customer",
    name: "customer",
    children: customerChildRoutes,
  },
  {
    path: "/:pathMatch(.*)",
    name: "notFound",
    component: PageNotFound,
  },
];
const router = createRouter({
  history: createWebHistory(),
  routes,
});
export default router;
