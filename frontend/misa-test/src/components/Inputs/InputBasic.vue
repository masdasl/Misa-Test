<script setup lang="ts">
const props = defineProps<{
  customBorder?: string;
  iconLeft?: string;
  customStyleIconLeft?: string;
  iconRight?: string;
  customStyleIconRight?: string;
  holderText?: string;
  customIcon?: boolean;
  customStyleInput?: string;
}>();
const emit = defineEmits(["change:inputSreach"]);

const placeholderText = props.holderText ?? "null";
// -------------------------------------------------------
// Công dụng: Nẵm bắt dữ liệu thay đổi cua input
// Đầu vào: value - string giá trị được nhập vào input
// Ai tạo: Phan Duy Anh
// Tạo ngày: 11/21/2025
// -------------------------------------------------------
function changeInput(value: string) {
  emit("change:inputSreach", value);
}
</script>
<template>
  <div
    :class="[
      'flex items-center  w-80  rounded p-1',
      props.customBorder || 'border border-gray-300',
    ]"
  >
    <span
      v-if="props.iconLeft"
      :class="[' icon', props.customStyleIconLeft, iconLeft]"
    >
    </span>

    <input
      type="text"
      :placeholder="placeholderText"
      @input="changeInput(($event.target as HTMLInputElement).value)"
      :class="[
        'flex-1 bg-transparent outline-none font-14',
        props.customStyleInput,
      ]"
    />
    <span
      v-if="props.iconRight"
      :class="['icon', props.customStyleIconLeft]"
    ></span>
    <slot v-if="props.customIcon"></slot>
  </div>
</template>
<style scoped>
input::placeholder {
  color: var(--primary-color);
  opacity: 1;
}
</style>
