import CustomerAdd from "./pages/customer-add/CustomerAdd.vue";
import CustomerDetail from "./pages/customer-detail/CustomerDetail.vue";
import CustomerEdit from "./pages/customer-edit/CustomerEdit.vue";

export const customerChildRoutes = [
  { path: "", name: "customer-detail", component: CustomerDetail },
  { path: "add", name: "customer-add", component: CustomerAdd },
  {
    path: "update/:customerId",
    name: "customer-edit",
    component: CustomerEdit,
  },
];
