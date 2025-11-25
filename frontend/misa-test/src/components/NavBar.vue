<script setup lang="ts">
const props = defineProps<{
  iconName: string;
  label: string;
  isActive?: boolean;
  link: string;
}>();
</script>

<template>
  <RouterLink
    :to="props.link"
    class="navbar-link flex items-center h-full py-3 cursor-pointer select-none"
    :class="{ active: props.isActive }"
  >
    <span
      :class="[
        'icon',
        props.iconName,
        props.isActive ? 'background-primary' : '',
      ]"
    >
    </span>
    <span class="font-14 pl-2 pt-1">
      {{ props.label }}
    </span>
  </RouterLink>
</template>

<style scoped>
.navbar-link {
  user-select: none;
  border-bottom: 1px solid transparent;
  color: inherit;
}

.navbar-link:hover {
  color: var(--primary-color);
  border-bottom: 2px solid var(--primary-color);
}

.navbar-link.active {
  color: var(--primary-color);
  border-bottom: 2px solid var(--primary-color);
}
</style>
