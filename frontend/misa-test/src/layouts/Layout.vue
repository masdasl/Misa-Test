<script setup lang="ts">
import BodyLayout from "./BodyLayout.vue";
import HeaderLayout from "./HeaderLayout.vue";
</script>
<template>
  <HeaderLayout />
  <BodyLayout>
    <RouterView />
  </BodyLayout>
</template>
<style scoped></style>
