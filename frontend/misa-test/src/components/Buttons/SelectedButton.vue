<script setup lang="ts">
import { ref } from "vue";
import type { selectedButtonModel } from "../../Model/selectedButtonModel";

const props = withDefaults(defineProps<selectedButtonModel>(), {
  isSelectedIcon: true,
  position: "top-10",
});
const emit = defineEmits<{
  (e: "seleted", value: string): void;
}>();
const isOpen = ref(false);
const selectedValue = ref(props.labelName);

function toggleDropdown() {
  isOpen.value = !isOpen.value;
}

function selectOption(option: string, name: string) {
  selectedValue.value = name;
  emit("seleted", option);
  isOpen.value = false;
}
</script>
<template>
  <div
    @click="toggleDropdown"
    class="relative inline-flex items-center bg-white rounded cursor-pointer border border-gray-300 p-1.75 max-h-8"
  >
    <span
      v-if="props.iconName"
      :class="['icon', props.iconName, selectedValue ? 'ml-1' : '']"
    ></span>
    <span
      v-if="props.labelName"
      :class="['font-14 font-bold ml-1.5', props.defaultWidth]"
      >{{ selectedValue }}</span
    >
    <span
      v-if="props.isSelectedIcon"
      class="icon icon-angle-down ml-1.5 mr-1"
    ></span>
    <div
      v-if="isOpen && props.isSelectedIcon && props.options?.length"
      :class="[
        'absolute left-0 w-full bg-white border border-gray-300 rounded shadow z-20',
        props.position,
      ]"
    >
      <div
        v-for="option in props.options"
        :key="option.value"
        @click.stop="selectOption(String(option.value), String(option.name))"
        class="p-2 hover:bg-gray-100 cursor-pointer"
      >
        {{ option.name }}
      </div>
    </div>
  </div>
</template>
<style scoped></style>
