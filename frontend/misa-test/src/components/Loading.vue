<template>
  <div v-if="visible" class="loading-overlay">
    <div class="spinner"></div>
  </div>
</template>

<script setup lang="ts">
import { defineProps } from "vue";

const props = defineProps<{
  visible: boolean;
}>();
</script>

<style scoped>
.loading-overlay {
  position: absolute;
  inset: 0;
  background: rgba(255, 255, 255, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 50;
}

.spinner {
  width: 50px;
  height: 50px;
  border: 5px solid #ccc;
  border-top-color: #6366f1;
  border-radius: 50%;
  animation: spin 1s linear infinite;
}

@keyframes spin {
  to {
    transform: rotate(360deg);
  }
}
</style>
