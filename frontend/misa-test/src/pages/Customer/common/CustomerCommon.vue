<script lang="ts" setup>
import { useRouter } from "vue-router";
import MisaButton from "../../../components/Buttons/MisaButton.vue";
import InputFormMisa from "../../../components/Inputs/InputFormMisa.vue";
import { inputItems } from "../constant/customerCommon";
import type { Customer } from "../Model/customer";
import { onMounted, ref, watch } from "vue";
import { CustomerService } from "../service/CustomerService";
import { ErrorNotification } from "../../../Helper/toastHelper";
import Loading from "../../../components/Loading.vue";
const props = defineProps<{
  customerId?: string;
}>();
const router = useRouter();
const formData = ref<Customer>({
  customer_code: "",
  full_name: "",
  phone: "",
  email: "",
  address: "",
  tax_code: "",
  customer_type: "",
  avatar: "",
});
const customerService = new CustomerService();
const fileInput = ref<HTMLInputElement | null>(null);
const formImg = ref<File>();
const isLoading = ref<boolean>(false);

onMounted(async () => {
  if (props.customerId) {
    loadCustomer(props.customerId);
  } else {
    const response = await customerService.createaCustomerCode();
    if (!response.data) return;
    formData.value.customer_code = response.data;
  }
});
watch(formData, (newVal) => {}, { deep: true });

const loadCustomer = async (id: string) => {
  try {
    isLoading.value = true;
    const response = await customerService.findCustomerId(id);
    if (response.data && !response.error) {
      formData.value = response.data;
      isLoading.value = false;
    } else {
      router.push({ name: "notFound", params: { pathMatch: id } });
    }
  } catch (err) {
    console.error(err);
  }
};

// -------------------------------------------------------
// Công dụng: Mở hộp thoại để chọn file
// Đầu vào:
// Ai tạo: Phan Duy Anh
// Tạo ngày: 11/19/2025
// -------------------------------------------------------
const openFileSelector = () => {
  fileInput.value?.click();
};

// -------------------------------------------------------
// Công dụng: Mở hộp thoại để chọn file
// Đầu vào:
// Ai tạo: Phan Duy Anh
// Tạo ngày: 11/19/2025
// -------------------------------------------------------
const handleFileChange = (event: Event) => {
  const target = event.target as HTMLInputElement;
  const file = target.files?.[0];
  if (!file) return;
  formImg.value = file;
  const reader = new FileReader();
  reader.onload = () => {
    formData.value.avatar = reader.result as string;
  };
  reader.readAsDataURL(file);
};

// -------------------------------------------------------
// Công dụng: Thêm hoặc update thông tin khách hàng
// Đầu vào:
// Ai tạo: Phan Duy Anh
// Tạo ngày: 11/19/2025
// -------------------------------------------------------
async function saveData() {
  isLoading.value = true;
  let filePath = formData.value.avatar;
  if (formImg.value) {
    const imageForm = new FormData();
    imageForm.append("image", formImg.value);
    const response = await customerService.uploadImage(imageForm);

    if (!response.error && response.data) {
      filePath = response.data;
    } else if (response.error) {
      ErrorNotification("Upload ảnh thất bại: " + response.error);
      return;
    }
  }
  const payload = {
    ...formData.value,
    avatarPath: filePath,
  };
  const response = !props.customerId
    ? await customerService.createCustomer([payload])
    : await customerService.editCustomer(props.customerId, payload);
  if (!response.error) {
    router.push({ name: "customer-detail" });
    isLoading.value = false;
  }
}
</script>
<template>
  <div class="flex flex-col h-screen">
    <Loading :visible="isLoading" />
    <div class="flex justify-between">
      <div class="flex items-center px-4">
        <span class="font-18 font-bold mr-3">{{
          customerId ? "Sửa thông tin Khách hàng" : "Thêm Khách hàng"
        }}</span>
        <span class="font-16 font-bold pt-1 ml-1">Mẫu tiêu chuẩn</span>
        <span class="material-symbols-outlined text-gray-500 mx-1.5 pt-1"
          >keyboard_arrow_down</span
        >
        <a href="" class="font-14 pt-1">Sửa bố cục</a>
      </div>
      <div class="flex py-3 px-1 gap-x-3 items-center">
        <MisaButton
          label-name="Hủy bỏ"
          :custom-button="true"
          custom-style-btn="border border-gray-300 bg-white text-black  px-3"
          @click="() => router.back()"
        />
        <MisaButton
          label-name="Lưu và thêm"
          :custom-button="true"
          custom-style-btn="border border-blue-500 bg-white text-primary relative px-3"
        />
        <MisaButton
          label-name="Lưu "
          :custom-button="true"
          @click="saveData"
          custom-style-btn=" background-primary text-white  px-3"
        />
      </div>
    </div>
    <div class="flex flex-1 bg-white pt-6 px-12 overflow-auto flex-col">
      <div class="font-bold font-16">Ảnh</div>
      <div class="flex flex-col mt-3 mb-6">
        <div
          class="w-12 h-12 rounded-full overflow-hidden cursor-pointer relative"
          @click="openFileSelector"
        >
          <img
            :src="
              formData.avatarPath ||
              formData.avatar ||
              '/public/images/avatar_default.png'
            "
            alt="User Avatar"
            class="w-full h-full object-cover"
          />
        </div>
        <input
          type="file"
          ref="fileInput"
          class="hidden"
          accept="image/*"
          @change="handleFileChange"
        />
      </div>
      <div class="text-base font-bold mb-3">Thông tin chung</div>
      <InputFormMisa :items="inputItems" v-model="formData" />
    </div>
  </div>
</template>
<style scope></style>
