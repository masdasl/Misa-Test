<script setup lang="ts">
import { onMounted, ref } from "vue";
import { ErrorNotification } from "../../../../Helper/toastHelper";
import { customerType } from "../../constant/customerType";
import InputBasic from "../../../../components/Inputs/InputBasic.vue";
import MisaButton from "../../../../components/Buttons/MisaButton.vue";
import MisaTable from "../../../../components/MisaTable.vue";
import SelectedButton from "../../../../components/Buttons/SelectedButton.vue";
import { useRouter } from "vue-router";
import type { PaginationData, Row } from "../../../../Model/misaTableModel";
import { pageSizeOptions } from "../../../../constant/MisaTable";
import { CustomerService } from "../../service/CustomerService";
import Loading from "../../../../components/Loading.vue";
import * as XLSX from "xlsx";
import { formatDate } from "../../../../Helper/ConvertData";
import { showConfirm } from "../../../../Helper/ConfirmModal";

const router = useRouter();
const customerService = new CustomerService();
const fileInput = ref<HTMLInputElement | null>(null);
const fileName = ref<string>();
const checkBoxList = ref<string[]>([]);
const valueUpdated = ref<string | null>(null);
const paginationData = ref<PaginationData>({
  pageNo: 1,
  pageSize: Number(pageSizeOptions[0]?.value),
  sortType: "DESC",
  sortKey: "",
  filters: {},
  searchValue: {},
});
const dataSource = ref<Row[]>([]);
const toltalItem = ref<number>(0);
const excelFormData = ref<FormData | null>(null);
const isLoading = ref(false);
onMounted(async () => {
  loadTableData();
});

const columns = [
  { dataIndex: "customer_id", type: "check" },
  { title: "Loại khách hàng", dataIndex: "customer_type", type: "link" },
  { title: "Mã khách hàng", dataIndex: "customer_code", type: "link" },
  { title: "Tên khách hàng ", dataIndex: "full_name", type: "label" },
  {
    title: "Mã số thuế",
    dataIndex: "tax_code",
    type: "label",
  },
  {
    title: "Địa chỉ (Giao hàng)",
    dataIndex: "shipping_address",
    type: "label",
  },
  { title: "Điện thoại ", dataIndex: "phone", type: "custom" },
  {
    title: "Ngày mua hàng gần nhất",
    dataIndex: "last_purchase_date",
    type: "date",
  },
  {
    title: "Hàng hóa đã mua",
    dataIndex: "purchased_items",
    type: "label",
  },
  {
    title: "Tên hàng hóa đã mua",
    dataIndex: "last_purchased_item",
    type: "label",
  },
  { title: "Email", dataIndex: "email", type: "label" },
  { title: "Zalo", dataIndex: "zalo", type: "label" },
  { title: "Địa chỉ", dataIndex: "address", type: "label" },
  { title: "Địa chỉ thanh toán", dataIndex: "billing_address", type: "label" },
];

// -------------------------------------------------------
// Công dụng: Load dữ liệu khách hàng từ API dựa trên paginationData.
//            Cập nhật dataSource và tổng số bản ghi.
// Đầu vào: Không có
// Ai tạo: Phan Duy Anh
// Tạo ngày: 11/21/2025
// -------------------------------------------------------
async function loadTableData() {
  isLoading.value = true;
  try {
    const response = await customerService.loadDataCustomerTable(
      paginationData.value
    );
    if (response.data) {
      dataSource.value = response.data.map((c) => ({
        ...c,
        key: c.customer_id,
      }));
    }
    toltalItem.value = response.meta?.total || 0;
  } catch (error) {
    console.error("Lỗi khi load dữ liệu khách hàng:", error);
  } finally {
    isLoading.value = false;
  }
}

// -------------------------------------------------------
// Công dụng: Xử lý sự kiện khi người dùng chọn file import Excel.
//            - Kiểm tra xem file có hợp lệ (xlsx, xls, csv).
//            - Nếu hợp lệ: lưu tên file và tạo FormData để gửi lên API.
//            - Nếu không hợp lệ: thông báo lỗi và reset input file.
// Đầu vào: e: Event - Sự kiện change từ input type="file".
// Ai tạo: Phan Duy Anh
// Tạo ngày: 11/25/2025
// -------------------------------------------------------
const handleButtonClick = () => {
  fileInput.value?.click();
};

const handleFileChange = (e: Event) => {
  const target = e.target as HTMLInputElement;
  const file = target.files?.[0];
  if (!file) return;
  const formFile = [".xlsx", ".xls", ".csv"];
  const fileType = file.name.slice(file.name.lastIndexOf("."));
  if (!formFile.includes(fileType.toLowerCase())) {
    ErrorNotification("Không phải file excel");
    fileName.value = "";
    target.value = "";
    return;
  }
  fileName.value = file.name;
  const formData = new FormData();
  formData.append("file", file);
  excelFormData.value = formData;
};

// -------------------------------------------------------
// Công dụng: Lấy dư liệu khách hàng theo nhóm
// Đầu vào: value:string - nhóm khách hàng muốn lấy
// Ai tạo: Phan Duy Anh
// Tạo ngày: 11/18/2025
// ---
function handleCustomerTypeChange(value: string) {
  if (value !== customerType[0]?.value) {
    paginationData.value.filters!["customer_type"] = value;
  } else {
    paginationData.value.filters = {};
  }
  paginationData.value.pageNo = 1;
  loadTableData();
}

// -------------------------------------------------------
// Công dụng: Nhận danh sách các key của các hàng được chọn từ bảng
//            và xử lý khi trạng thái checkbox thay đổi.
// Đầu vào: values: string[] - Mảng các key của các hàng đang được chọn.
// Ai tạo: Phan Duy Anh
// Tạo ngày: 11/18/2025
// ---
function checkboxValues(values: string[]) {
  checkBoxList.value = values;
}

// -------------------------------------------------------
// Công dụng: Nhận key của dòng được click trực tiếp (chọn 1 dòng).
//            Dùng cho chức năng SỬA vì chỉ được sửa 1 dòng tại 1 thời điểm.
//            Nếu giá trị null → nghĩa là bỏ chọn dòng.
// Đầu vào: values: string | null - Key của dòng được chọn hoặc null.
// Ai tạo: Phan Duy Anh
// Tạo ngày: 11/18/2025
// -------------------------------------------------------
function seletedUpdateValue(values: string | null) {
  valueUpdated.value = values;
  if (valueUpdated.value) {
    const customerId = valueUpdated.value;
    router.push({ name: "customer-edit", params: { customerId } });
  }
}

// -------------------------------------------------------
// Công dụng: Xử lý khi thay đổi trang (pagination).
// Đầu vào: value: number - Số trang hiện tại sau khi thay đổi.
// Ai tạo: Phan Duy Anh
// Tạo ngày: 11/21/2025
// -------------------------------------------------------
function onPageChanged(value: number) {
  paginationData.value.pageNo = value;
  loadTableData();
}

// -------------------------------------------------------
// Công dụng: sắp xếp theo key header table.
// Đầu vào: value: key đang được chọn và sắp xếp
// Ai tạo: Phan Duy Anh
// Tạo ngày: 11/21/2025
// -------------------------------------------------------
function sortTable(value: string, isSort: boolean) {
  paginationData.value.sortType = isSort ? "DESC" : "ASC";
  paginationData.value.sortKey = value;
  loadTableData();
}

// -------------------------------------------------------
// Công dụng: Xử lý khi thay đổ số lượng items trên table.
// Đầu vào: value: number - số lượng items.
// Ai tạo: Phan Duy Anh
// Tạo ngày: 11/21/2025
// -------------------------------------------------------
function onPageSizeChanged(value: number) {
  const oldPageNo = paginationData.value.pageNo;
  const oldPageSize = paginationData.value.pageSize;
  const firstRecordIndex = (oldPageNo - 1) * oldPageSize;
  const newPageNo = Math.floor(firstRecordIndex / value) + 1;
  paginationData.value.pageSize = value;
  paginationData.value.pageNo = newPageNo;
  loadTableData();
}

// -------------------------------------------------------
// Công dụng: Xử lý các thao tác trên khách hàng: Thêm, Sửa, Xóa.
//            - "edit": chuyển đến trang sửa dựa trên customerId.
//            - "delete": gọi API xóa khách hàng dựa trên customerId.
// Đầu vào:
//    action: "add" | "edit" | "delete"
//        - "add": điều hướng sang customer-add
//        - "edit": điều hướng sang customer-edit với params customerId
//        - "delete": xóa khách hàng thông qua API
// Ai tạo: Phan Duy Anh
// Tạo ngày: 11/18/2025
// -------------------------------------------------------
async function handleCustomerAction(action: string) {
  switch (action) {
    case "import-excel":
      if (!excelFormData.value) return;

      const importResult = await showConfirm({
        title: "Nhập file Excel",
        message: `Bạn có chắc chắn muốn lưu file ${fileName.value} không?`,
      });

      if (!importResult.isConfirmed) return;

      isLoading.value = true;
      try {
        const response = await customerService.importCustomers(
          excelFormData.value
        );
        if (response.data && response.error) {
          await showImportErrors(response.data);
        }
      } finally {
        excelFormData.value = null;
        isLoading.value = false;
      }
      break;

    case "delete":
      const deleteResult = await showConfirm({
        title: "Xóa khách hàng",
        message: `Bạn có chắc chắn muốn xóa không?`,
      });
      if (!deleteResult.isConfirmed) return;

      await customerService.deleteCustomer(checkBoxList.value);
      valueUpdated.value = null;
      checkBoxList.value = [];
      break;

    case "export-excel":
      const exportResult = await showConfirm({
        title: "Xuất Excel",
        message: "Bạn có chắc chắn muốn lưu file excel không?",
      });
      if (!exportResult.isConfirmed) return;

      exportSelectedToExcel();
      break;

    default:
      console.warn("Không xác định action:", action);
  }

  await loadTableData();
}

// -------------------------------------------------------
// Công dụng:Hiển thị các lỗi khi import file excel dưới dạng table
// Đầu vào:errors là mảng danh sách lỗi (kiểu giá trị dữ liệu là key:value)
// Ai tạo: Phan Duy Anh
// Tạo ngày: 11/25/2025
// -------------------------------------------------------
async function showImportErrors(errors: Array<Record<string, string>>) {
  if (!errors || errors.length === 0) return;
  const firstError = errors[0] || {};
  const errorKeys = Object.keys(firstError).filter((key) =>
    errors.some((err) => err[key] && err[key] !== "")
  );
  const totalError = errors.length;
  const displayColumns = columns.filter((col) =>
    errorKeys.includes(col.dataIndex)
  );
  let html = `<div class="max-h-72 overflow-y-auto">`;
  html += `<table class="min-w-max divide-y divide-gray-200 w-full border border-gray-200 rounded">`;
  html += `<thead class="bg-gray-50 sticky top-0 z-10 cursor-pointer"><tr>`;
  displayColumns.forEach((col) => {
    html += `<th class="py-2 px-4 font-bold text-left">${col.title}</th>`;
  });
  html += `</tr></thead>`;
  html += `<tbody class="bg-white divide-y divide-gray-200 border-b border-gray-200">`;
  errors.forEach((err) => {
    html += `<tr class="cursor-pointer font-14 hover:bg-blue-50">`;
    displayColumns.forEach((col) => {
      html += `<td class="py-2 px-4 text-left">${
        err[col.dataIndex] ?? ""
      }</td>`;
    });
    html += `</tr>`;
  });
  html += `</tbody></table></div>`;

  await showConfirm({
    title: `Có ${totalError} lỗi`,
    customerModel: html,
    confirmText: "Đóng",
    isHiddenCancelBtn: true,
  });
}

// -------------------------------------------------------
// Công dụng: Lưu dư liêu đã chọn trên table vào file excel
// Ai tạo: Phan Duy Anh
// Tạo ngày: 11/23/2025
// -------------------------------------------------------
function exportSelectedToExcel() {
  const exportData = dataSource.value
    .filter((row) => checkBoxList.value.includes(row.customer_id))
    .map((row) => {
      const obj: Record<string, any> = {};
      columns.forEach((col) => {
        if (col.type !== "check" && col.title) {
          let value = row[col.dataIndex?.trim() || ""];

          if (col.type === "date") {
            value = formatDate(value);
          }

          obj[col.title] = value ?? "";
        }
      });
      return obj;
    });

  const ws = XLSX.utils.json_to_sheet(exportData);
  const wb = XLSX.utils.book_new();
  XLSX.utils.book_append_sheet(wb, ws, "KhachHang");
  XLSX.writeFile(wb, `Danh_sach_khach_hang.xlsx`);
}

// -------------------------------------------------------
// Công dụng: Tìm kiếm thông tin khách hàng trong bảng
// Đầu vào: value - string được chuyền vào khi input search được thay đổi
// Ai tạo: Phan Duy Anh
// Tạo ngày: 11/21/2025
// -------------------------------------------------------
function searchCustomerTable(value: string) {
  paginationData.value.searchValue!["email"] = value;
  paginationData.value.searchValue!["phone"] = value;
  paginationData.value.searchValue!["full_name"] = value;
  paginationData.value.pageNo = 1;
  loadTableData();
}
</script>
<template>
  <div class="customer-container flex flex-col h-full">
    <Loading :visible="isLoading" />
    <div class="customer-container-header flex p-3.5 justify-between">
      <div class="customer-container-header-left flex items-center">
        <SelectedButton
          v-if="checkBoxList.length === 0"
          icon-name="icon-folder"
          :label-name="customerType[0]?.name"
          :options="customerType"
          default-width="w-31"
          @seleted="handleCustomerTypeChange"
        />
        <MisaButton
          v-if="checkBoxList.length > 0"
          label-name="Xóa"
          icon-name="icon-delete"
          custom-icon="background-primary"
          custom-style-btn="border border-blue-500 bg-white text-primary"
          @click="handleCustomerAction('delete')"
        />
        <MisaButton
          v-if="excelFormData"
          icon-name="icon-import"
          custom-icon="background-primary"
          label-name="Import file excel"
          custom-style-btn="border border-blue-500 bg-white text-primary pr-2 ml-2.5"
          @click="handleCustomerAction('import-excel')"
        />
        <MisaButton
          v-if="checkBoxList.length > 0"
          label-name="Export file excel"
          icon-name="icon-export"
          custom-icon="background-primary"
          custom-style-btn="border border-blue-500 bg-white text-primary pr-2 ml-2.5"
          @click="handleCustomerAction('export-excel')"
        />
        <a href="" class="font-14 mx-4">Sửa</a>
        <span class="icon icon-reload"> </span>
      </div>
      <div class="customer-container-header-right flex items-center gap-2.5">
        <InputBasic
          :custom-icon="true"
          icon-left="icon-smart-search"
          holder-text="Tìm kiếm thông minh"
          custom-style-icon-left="text-purple-500 opacity-70 mx-1"
          custom-border="gradient-border-box"
          custom-style-input="text-primary "
          @change:input-sreach="searchCustomerTable"
        >
          <img src="/images/icons/icon-ai.svg" alt="" class="mx-1.5 my-1" />
        </InputBasic>
        <div class="gradient-border-box p-2">
          <span class="icon icon-statistic"> </span>
        </div>

        <MisaButton
          label-name="Thêm"
          icon-name="icon-plus"
          custom-icon="!bg-white"
          custom-style-btn="background-primary  text-white pr-2"
          @click="() => router.push({ name: 'customer-add' })"
        />
        <MisaButton
          label-name="Nhập từ excel"
          icon-name="icon-import"
          custom-icon="background-primary"
          :custom-button="true"
          custom-style-btn="border border-blue-500 bg-white text-primary relative pr-2"
          @click="handleButtonClick"
        >
          <input
            ref="fileInput"
            type="file"
            class="hidden"
            accept=".xlsx,.xls,.csv"
            @change="handleFileChange"
          />
        </MisaButton>
        <SelectedButton icon-name=" icon-dot-menu" :isSelectedIcon="false" />
        <SelectedButton icon-name=" icon-category" />
      </div>
    </div>
    <div class="container-body bg-white">
      <MisaTable
        :toltal-item="toltalItem"
        :columns="columns"
        :data-source="dataSource"
        :pagination="false"
        :page-no="paginationData.pageNo"
        :scroll="{ y: 530, x: 1200 }"
        @change:current-page="onPageChanged"
        @change:page-size="onPageSizeChanged"
        @change:selected-row-keys-value="checkboxValues"
        @click:selectedRowKey="seletedUpdateValue"
        @sort:sort-table="sortTable"
      >
        <template #customType="{ row, col }">
          <div class="flex items-center gap-2">
            <img
              src="/public/images/icons/icon-phone.png"
              alt=""
              class="w-4 h-4 mb-1"
            />
            <span class="text-primary font-14">{{
              `0${row[col.dataIndex]}`
            }}</span>
          </div>
        </template>
      </MisaTable>
    </div>
  </div>
</template>
<style scoped>
.gradient-border-box {
  position: relative;
  background: linear-gradient(to right, rgb(237, 239, 254), #ffffff);
  z-index: 0;
  border-radius: 3px;
}
.gradient-border-box::before {
  content: "";
  position: absolute;
  inset: 0;
  padding: 1px;
  border-radius: 3px;
  background: linear-gradient(to right, #a78bfa, var(--primary-color));
  mask: conic-gradient(#000 0 0) content-box exclude, conic-gradient(#000 0 0);
  z-index: -1;
}
.custom-phone-icon {
  width: 14px;
  height: 14px;
  font-size: 14px;
  line-height: 1;
}
</style>
