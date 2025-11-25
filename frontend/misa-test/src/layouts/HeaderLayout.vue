<script setup lang="ts">
import NavBar from "../components/NavBar.vue";
import { navbarItems } from "../constant/navbarItems";
import { useRoute } from "vue-router";

const route = useRoute();
</script>

<template>
  <div
    class="bg-white flex items-center pl-8 shadow-sm h-full header-container max-h-12"
  >
    <div class="header-left flex items-center gap-3">
      <div class="header-icon flex items-center">
        <RouterLink to="/">
          <img src="/public/images/logo-crm.svg" alt="Logo" />
        </RouterLink>
        <span class="text-lg font-bold my-3 mx-2">CRM</span>
      </div>
      <div class="navbar-container flex items-center gap-4">
        <NavBar
          v-for="(item, index) in navbarItems"
          :key="index"
          :icon-name="item.iconName"
          :label="item.label"
          :isActive="route.path === item.link"
          :link="item.link"
        />
      </div>
    </div>
    <div class="header-right flex items-center ml-auto pr-3">
      <img class="w-8 h-8" src="/public/images/avatar_user.png" alt="Logo" />
    </div>
  </div>
</template>

<style scoped></style>
