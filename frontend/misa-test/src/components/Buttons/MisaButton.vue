<script setup lang="ts">
const props = defineProps<{
  iconName?: string;
  labelName?: string;
  customStyleBtn?: string;
  customButton?: boolean;
  customIcon?: string;
}>();
const emit = defineEmits<{
  (e: "click"): void;
}>();

const handleClick = () => {
  emit("click");
};
</script>
<template>
  <div
    :class="[
      'flex items-center justify-center rounded  cursor-pointer py-1.5 max-h-8',
      props.customStyleBtn || 'border border-blue-500 bg-white text-primary ',
    ]"
    @click="handleClick"
  >
    <span
      v-if="props.iconName"
      :class="['  font-14 mx-2 icon', props.customIcon, props.iconName]"
    >
    </span>
    <span
      v-if="props.labelName"
      :class="[' font-14 font-bold', props.iconName ? 'pr-2' : '']"
      >{{ props.labelName }}</span
    >
    <slot v-if="props.customButton"></slot>
  </div>
</template>
<style scoped></style>
