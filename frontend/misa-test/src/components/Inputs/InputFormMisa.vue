<script setup lang="ts">
import { reactive } from "vue";
import type { InpurForm } from "../../Model/InputForm";
import { CustomerService } from "../../pages/Customer/service/CustomerService";
import type { ApiResponse } from "../../Model/ApiResponse";
import { ErrorNotification } from "../../Helper/toastHelper";

const props = defineProps<{
  items: InpurForm[];
  modelValue: any;
}>();
const emit = defineEmits(["update:modelValue"]);
const customerService = new CustomerService();
const showError = reactive<{ [key: string]: string | null }>({});
const leftColumn = props.items.filter((_, i) => i % 2 === 0);
const rightColumn = props.items.filter((_, i) => i % 2 === 1);

// -------------------------------------------------------
// Công dụng: Truyền giá trị được cập nhập lên parent component
// Đầu vào:
//     key   - Tên của trường cần cập nhật (string)
//     value - Giá trị mới của trường (any)
// Ai tạo: Phan Duy Anh
// Tạo ngày: 11/19/2025
// -------------------------------------------------------
function updateValue(key: string, value: any) {
  emit("update:modelValue", {
    ...props.modelValue,
    [key]: value,
    error: showError[key],
  });
}

// -------------------------------------------------------
// Công dụng: Kiểm tra email có đúng format ko
// Đầu vào:
//     key   - Tên của trường cần cập nhật (string)
//     value - Giá trị mới của trường (any)
// Ai tạo: Phan Duy Anh
// Tạo ngày: 11/19/2025
// -------------------------------------------------------
function isValidEmail(email: string): boolean {
  const regex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
  return regex.test(email);
}

// -------------------------------------------------------
// Công dụng: Kiểm tra dữ liệu nhập vào và check xem các trường require có bị trống không
// Đầu vào:
//     key   - Tên của trường cần cập nhật (string)
//     value - Giá trị mới của trường (any)
// Ai tạo: Phan Duy Anh
// Tạo ngày: 11/19/2025
// -------------------------------------------------------
async function checkRequire(key: string, value: any, type?: string) {
  if (!value) {
    const item = props.items.find((i) => i.key === key);
    const fieldLabel = item?.label || key;
    showError[key] = `${fieldLabel} là bắt buộc`;
    return;
  }
  if (type === "email" && !isValidEmail(value)) {
    showError[key] = "Email không hợp lệ";
    return;
  }
  if (type === "email") {
    if (!isValidEmail(value)) {
      showError[key] = "Email không hợp lệ";
      return;
    }
    const response: ApiResponse<string> = await customerService.checkEmail(
      value
    );
    if (response.error !== null) {
      showError[key] = "Email đã tồn tại";
      ErrorNotification(String(response.error));
      return;
    }
  }
  if (type === "phone") {
    const phoneRegex = /^\d{10,11}$/;
    if (!phoneRegex.test(value)) {
      showError[key] = "Số điện thoại không hợp lệ";
      return;
    }
    const response: ApiResponse<string> = await customerService.checkPhone(
      value
    );
    if (response.error !== null) {
      showError[key] = "Số điện thoại đã tồn tại";
      ErrorNotification(String(response.error));
      return;
    }
  }
  showError[key] = null;
}
</script>

<template>
  <div class="grid grid-cols-2 gap-20">
    <!-- Cột trái -->
    <div>
      <component :is="'div'" v-for="item in leftColumn" :key="item.key">
        <template
          v-if="
            item.type === 'input' ||
            item.type === 'email' ||
            item.type === 'phone'
          "
        >
          <div class="flex items-center mb-3 relative">
            <label class="w-39 font-14" :for="item.key">
              {{ item.label }}
              <span v-if="item.require" class="text-red-500">*</span>
            </label>
            <input
              :id="item.key"
              type="text"
              :class="[
                'border border-gray-300 rounded px-3  flex-grow focus:outline-none focus:ring-0 focus:border-gray-300',
                showError[item.key] ? 'border-red-500' : '',
              ]"
              :value="props.modelValue[item.key]"
              :disabled="item.isDisabled"
              @input="
                updateValue(item.key, ($event.target as HTMLInputElement).value)
              "
              @blur="
                item.require &&
                  checkRequire(
                    item.key,
                    ($event.target as HTMLInputElement).value,
                    item.type
                  )
              "
            />
            <span
              v-if="showError[item.key]"
              class="text-red-500 text-xs absolute top-6"
            >
              {{ showError[item.key] }}
            </span>
          </div>
        </template>

        <!-- render date -->
        <template v-if="item.type === 'date'">
          <div class="flex items-center mb-3 relative">
            <label class="w-39 font-14" :for="item.key">
              {{ item.label }}
              <span v-if="item.require" class="text-red-500">*</span>
            </label>
            <input
              :id="item.key"
              type="date"
              :class="[
                'border border-gray-300 rounded px-3  flex-grow focus:outline-none focus:ring-0 focus:border-gray-300',
                showError[item.key] ? 'border-red-500' : '',
              ]"
              :value="
                props.modelValue[item.key] ||
                new Date().toISOString().split('T')[0]
              "
              :disabled="item.isDisabled"
              @input="
                updateValue(item.key, ($event.target as HTMLInputElement).value)
              "
              @blur="
                item.require &&
                  checkRequire(
                    item.key,
                    ($event.target as HTMLInputElement).value,
                    item.type
                  )
              "
            />
            <span
              v-if="showError[item.key]"
              class="text-red-500 text-xs absolute top-6 left-10"
            >
              {{ showError[item.key] }}
            </span>
          </div>
        </template>
        <!-- render textarea -->
        <template v-if="item.type === 'textarea'">
          <div class="flex items-start mb-3 relative">
            <label class="w-39 pt-1 font-14" :for="item.key">
              {{ item.label }}
              <span v-if="item.require" class="text-red-500">*</span>
            </label>
            <textarea
              :id="item.key"
              rows="3"
              :class="[
                'border border-gray-300 rounded px-3 py-1 flex-grow font-14 resize-none focus:outline-none focus:ring-0 focus:border-gray-300',
                showError[item.key] ? 'border-red-500' : '',
              ]"
              :value="props.modelValue[item.key]"
              :disabled="item.isDisabled"
              @input="
                updateValue(item.key, ($event.target as HTMLInputElement).value)
              "
              @blur="
                item.require &&
                  checkRequire(
                    item.key,
                    ($event.target as HTMLInputElement).value,
                    item.type
                  )
              "
            ></textarea>
            <span
              v-if="showError[item.key]"
              class="text-red-500 text-xs absolute top-6"
            >
              {{ showError[item.key] }}
            </span>
          </div>
        </template>

        <!-- render select -->
        <template v-if="item.type === 'select'">
          <div class="flex items-center mb-3 relative">
            <label class="w-39 font-14" :for="item.key">
              {{ item.label }}
              <span v-if="item.require" class="text-red-500">*</span>
            </label>
            <div class="relative flex-grow">
              <select
                :id="item.key"
                :value="modelValue[item.key]"
                :disabled="item.isDisabled"
                :class="[
                  'w-full border  border-gray-300 rounded px-3 py-1 font-14 pr-8 focus:outline-none cursor-pointer focus:outline-none focus:ring-0 focus:border-gray-300',
                  showError[item.key] ? 'border-red-500' : '',
                ]"
                @input="
                  updateValue(
                    item.key,
                    ($event.target as HTMLInputElement).value
                  )
                "
                @blur="
                  item.require &&
                    checkRequire(
                      item.key,
                      ($event.target as HTMLInputElement).value,
                      item.type
                    )
                "
              >
                <option value="">-- Chọn loại --</option>
                <option
                  v-for="op in item.options"
                  :key="op.value"
                  :value="op.value"
                >
                  {{ op.name }}
                </option>
              </select>
              <span
                class="absolute top-1/5 right-1/55 flex items-center pointer-events-none text-gray-500 icon icon-angle-down"
              >
              </span>
            </div>
            <span
              v-if="showError[item.key]"
              class="text-red-500 text-xs absolute top-6"
            >
              {{ showError[item.key] }}
            </span>
          </div>
        </template>
      </component>
    </div>

    <!-- Cột phải -->
    <div>
      <component :is="'div'" v-for="item in rightColumn" :key="item.key">
        <template
          v-if="
            item.type === 'input' ||
            item.type === 'email' ||
            item.type === 'phone'
          "
        >
          <div class="flex items-center mb-3 relative">
            <label class="w-39 font-14" :for="item.key">
              {{ item.label }}
              <span v-if="item.require" class="text-red-500">*</span>
            </label>
            <input
              :id="item.key"
              type="text"
              :disabled="item.isDisabled"
              :class="[
                'border border-gray-300 rounded px-3  flex-grow focus:outline-none focus:ring-0 focus:border-gray-300',
                showError[item.key] ? 'border-red-500' : '',
              ]"
              :value="props.modelValue[item.key]"
              @input="
                updateValue(item.key, ($event.target as HTMLInputElement).value)
              "
              @blur="
                item.require &&
                  checkRequire(
                    item.key,
                    ($event.target as HTMLInputElement).value,
                    item.type
                  )
              "
            />
            <span
              v-if="showError[item.key]"
              class="text-red-500 text-xs absolute top-6"
            >
              {{ showError[item.key] }}
            </span>
          </div>
        </template>

        <!-- render date -->
        <template v-if="item.type === 'date'">
          <div class="flex items-center mb-3 relative">
            <label class="w-39 font-14" :for="item.key">
              {{ item.label }}
              <span v-if="item.require" class="text-red-500">*</span>
            </label>
            <input
              :id="item.key"
              type="date"
              :class="[
                'border border-gray-300 rounded px-3  flex-grow focus:outline-none focus:ring-0 focus:border-gray-300',
                showError[item.key] ? 'border-red-500' : '',
              ]"
              :value="
                props.modelValue[item.key] ||
                new Date().toISOString().split('T')[0]
              "
              :disabled="item.isDisabled"
              @input="
                updateValue(item.key, ($event.target as HTMLInputElement).value)
              "
              @blur="
                item.require &&
                  checkRequire(
                    item.key,
                    ($event.target as HTMLInputElement).value,
                    item.type
                  )
              "
            />
            <span
              v-if="showError[item.key]"
              class="text-red-500 text-xs absolute top-6 left-10"
            >
              {{ showError[item.key] }}
            </span>
          </div>
        </template>
        <!-- render textarea -->
        <template v-if="item.type === 'textarea'">
          <div class="flex items-start mb-3 relative">
            <label class="w-39 pt-1 font-14" :for="item.key">
              {{ item.label }}
              <span v-if="item.require" class="text-red-500">*</span>
            </label>
            <textarea
              :id="item.key"
              rows="3"
              :disabled="item.isDisabled"
              :class="[
                'border border-gray-300 rounded px-3 py-1 flex-grow font-14 resize-none focus:outline-none focus:ring-0 focus:border-gray-300',
                showError[item.key] ? 'border-red-500' : '',
              ]"
              :value="props.modelValue[item.key]"
              @input="
                updateValue(item.key, ($event.target as HTMLInputElement).value)
              "
              @blur="
                item.require &&
                  checkRequire(
                    item.key,
                    ($event.target as HTMLInputElement).value,
                    item.type
                  )
              "
            ></textarea>
            <span
              v-if="showError[item.key]"
              class="text-red-500 text-xs absolute top-6"
            >
              {{ showError[item.key] }}
            </span>
          </div>
        </template>

        <!-- render select -->
        <template v-if="item.type === 'select'">
          <div class="flex items-center mb-3 relative">
            <label class="w-39 font-14" :for="item.key">
              {{ item.label }}
              <span v-if="item.require" class="text-red-500">*</span>
            </label>
            <div class="relative flex-grow">
              <select
                :id="item.key"
                :disabled="item.isDisabled"
                :value="modelValue[item.key]"
                :class="[
                  'w-full border border-gray-300 rounded px-3 py-1 font-14 pr-8 focus:outline-none cursor-pointer focus:outline-none focus:ring-0 focus:border-gray-300',
                  showError[item.key] ? 'border-red-500' : '',
                ]"
                @input="
                  updateValue(
                    item.key,
                    ($event.target as HTMLInputElement).value
                  )
                "
                @blur="
                  item.require &&
                    checkRequire(
                      item.key,
                      ($event.target as HTMLInputElement).value,
                      item.type
                    )
                "
              >
                <option value="">-- Không chọn --</option>
                <option
                  v-for="op in item.options"
                  :key="op.value"
                  :value="op.value"
                >
                  {{ op.name }}
                </option>
              </select>
              <span
                class="absolute top-1/5 right-1/55 flex items-center pointer-events-none text-gray-500 icon icon-angle-down"
              >
              </span>
            </div>
            <span
              v-if="showError[item.key]"
              class="text-red-500 text-xs absolute top-6"
            >
              {{ showError[item.key] }}
            </span>
          </div>
        </template>
      </component>
    </div>
  </div>
</template>
