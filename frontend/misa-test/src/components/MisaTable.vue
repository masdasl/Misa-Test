<script lang="ts" setup>
import { computed, ref, watch } from "vue";
import SelectedButton from "./Buttons/SelectedButton.vue";
import type { Column, Row } from "../Model/misaTableModel";
import { pageSizeOptions } from "../constant/MisaTable";
import { formatDate } from "../Helper/ConvertData";

const props = defineProps<{
  columns: Column[];
  dataSource: Row[];
  toltalItem: number;
  scroll?: { x?: number | string; y?: number | string };
  pageNo?: number;
}>();
const emit = defineEmits<{
  (e: "change:currentPage", value: number): void;
  (e: "change:pageSize", value: number): void;
  (e: "change:selectedRowKeysValue", value: string[]): void;
  (e: "click:selectedRowKey", value: string | null): void;
  (e: "sort:sortTable", value: string, isSort: boolean): void;
}>();
const selectedRowKeys = ref<string[]>([]);
const pageSize = ref<number>(Number(pageSizeOptions[0]?.value));
const current = ref(props.pageNo || 1);
const sortType = ref<boolean>(true);
const selectedRowKey = ref<string | null>(null);
const activeSortKey = ref<string | null>(null);
watch(
  () => props.pageNo,
  (val) => {
    if (val) current.value = val;
  }
);

// -------------------------------------------------------
// Công dụng: Chọn hoặc bỏ chọn tất cả các hàng trong bảng
// Đầu vào: Không có
// Ai tạo: Phan Duy Anh
// Tạo ngày: 11/18/2025
// -------------------------------------------------------
const toggleAllRows = () => {
  const currentPageKeys = props.dataSource.map((d) => String(d.key));
  const allSelected = currentPageKeys.every((key) =>
    selectedRowKeys.value.includes(key)
  );

  if (allSelected) {
    selectedRowKeys.value = selectedRowKeys.value.filter(
      (key) => !currentPageKeys.includes(key)
    );
  } else {
    currentPageKeys.forEach((key) => {
      if (!selectedRowKeys.value.includes(key)) selectedRowKeys.value.push(key);
    });
  }

  emit("change:selectedRowKeysValue", selectedRowKeys.value);
};
// -------------------------------------------------------
// CCông dụng: Thay đổi số bản ghi hiển thị trên mỗi trang
// Đầu vào: value: string (giá trị pageSize mới)
// Ai tạo: Phan Duy Anh
// Tạo ngày: 11/18/2025
// -------------------------------------------------------
function handleChangePageSize(value: string) {
  pageSize.value = Number(value);
  emit("change:pageSize", pageSize.value);
}

// -------------------------------------------------------
// Công dụng: Chọn hoặc bỏ chọn một hàng theo key
// Đầu vào: key: string (key của hàng cần toggle)
// Ai tạo: Phan Duy Anh
// Tạo ngày: 11/18/2025
// -------------------------------------------------------
const toggleRowSelection = (key: string) => {
  if (selectedRowKeys.value.includes(key)) {
    selectedRowKeys.value = selectedRowKeys.value.filter((k) => k !== key);
  } else {
    selectedRowKeys.value.push(key);
  }
  emit("change:selectedRowKeysValue", selectedRowKeys.value);
};

// -------------------------------------------------------
// Công dụng: Tính tổng số trang dựa trên dataSource và pageSize
// Đầu vào: Không có
// Ai tạo: Phan Duy Anh
// Tạo ngày: 11/18/2025
// -------------------------------------------------------
const totalPages = computed(() => Math.ceil(props.toltalItem / pageSize.value));

// -------------------------------------------------------
// Công dụng:  Tính số bản ghi  bắt đầu của trang hiện tại
// Đầu vào: Không có
// Ai tạo: Phan Duy Anh
// Tạo ngày: 11/18/2025
// -------------------------------------------------------
const startRecord = computed(() => {
  return (current.value - 1) * pageSize.value + 1;
});

// -------------------------------------------------------
// Công dụng:  Tính số bản ghi kết thúc của trang hiện tại
// Đầu vào: Không có
// Ai tạo: Phan Duy Anh
// Tạo ngày: 11/18/2025
// -------------------------------------------------------
const endRecord = computed(() => {
  const end = current.value * pageSize.value;
  return end > props.toltalItem ? props.toltalItem : end;
});

// -------------------------------------------------------
// Công dụng:  Chuyển về trang trước
// Đầu vào: Không có
// Ai tạo: Phan Duy Anh
// Tạo ngày: 11/18/2025
// -------------------------------------------------------
const prevPage = () => {
  if (current.value > 1) {
    current.value--;
    emit("change:currentPage", current.value);
  }
};

// -------------------------------------------------------
// Công dụng:  Chuyển sang trang tiếp theo
// Đầu vào: Không có
// Ai tạo: Phan Duy Anh
// Tạo ngày: 11/18/2025
// -------------------------------------------------------
const nextPage = () => {
  if (current.value < totalPages.value) {
    current.value++;
    emit("change:currentPage", current.value);
  }
};

// -------------------------------------------------------
// Công dụng:  Chuyển về trang đầu
// Đầu vào: Không có
// Ai tạo: Phan Duy Anh
// Tạo ngày: 11/18/2025
// -------------------------------------------------------
const fistPage = () => {
  if (current.value > 1) {
    current.value = 1;
    emit("change:currentPage", current.value);
  }
};

// -------------------------------------------------------
// Công dụng: Chuyển về trang cuối
// Đầu vào: Không có
// Ai tạo: Phan Duy Anh
// Tạo ngày: 11/18/2025
// -------------------------------------------------------
const lastPage = () => {
  if (current.value < totalPages.value) {
    current.value = totalPages.value;
    emit("change:currentPage", current.value);
  }
};

// -------------------------------------------------------
// Công dụng: Chọn hoặc bỏ chọn hàng được click (toggle theo key)
// Đầu vào: value: string (key của hàng được click)
// Ai tạo: Phan Duy Anh
// Tạo ngày: 11/18/2025
// -------------------------------------------------------
function selectedItemUpdate(value: string, type: string) {
  if (type === "check") return;
  selectedRowKey.value = selectedRowKey.value === value ? null : value;
  emit("click:selectedRowKey", selectedRowKey.value);
}

// -------------------------------------------------------
// Công dụng: cho biết đang sort theo giá trị key nào
// Đầu vào: value: string (key của hàng được sort)
// Ai tạo: Phan Duy Anh
// Tạo ngày: 11/18/2025
// -------------------------------------------------------
function sortTableBySortKey(value: string, isSort: boolean) {
  activeSortKey.value = value;
  sortType.value = !isSort;
  selectedRowKeys.value = [];
  emit("sort:sortTable", value, sortType.value);
  emit("change:selectedRowKeysValue", selectedRowKeys.value);
}
</script>
<template>
  <div
    class="table-container border border-gray-200 rounded flex flex-col"
    :style="{ height: props.scroll?.y + 'px' }"
  >
    <div class="table-scroll overflow-auto flex-1">
      <table class="min-w-max divide-y divide-gray-200 w-full">
        <!-- Header Table -->
        <thead class="bg-gray-50 sticky top-0 z-10 cursor-pointer">
          <tr>
            <th
              v-for="col in props.columns"
              :key="col.dataIndex"
              :class="[
                'py-1 font-bold text-left',
                col.type !== 'check' ? 'px-4' : 'pl-11',
              ]"
            >
              <template v-if="col.type === 'check'">
                <input
                  class="w-3.5 h-3.5 mt-2"
                  type="checkbox"
                  :checked="
                    props.dataSource.length > 0 &&
                    props.dataSource.every((row) =>
                      selectedRowKeys.includes(String(row.key))
                    )
                  "
                  @change="toggleAllRows"
                />
              </template>

              <template v-else-if="col.type !== 'check'">
                <div
                  class="flex items-center gap-1 title-table-header"
                  @click="sortTableBySortKey(col.dataIndex, sortType)"
                >
                  <span class="font-14">{{ col.title }}</span>
                  <span
                    class="icon icon-angle-down font-14 mt-0.5 opacity-0"
                    :class="[
                      activeSortKey === col.dataIndex
                        ? 'opacity-100'
                        : 'opacity-0',
                      activeSortKey === col.dataIndex && !sortType
                        ? 'rotate-180'
                        : '',
                    ]"
                  ></span>
                </div>
              </template>
            </th>
          </tr>
        </thead>

        <!-- Body Table -->
        <tbody
          class="bg-white divide-y divide-gray-200 border-b border-gray-200"
        >
          <tr
            v-for="row in props.dataSource"
            :key="row.key"
            :class="[
              'cursor-pointer font-14 hover:bg-blue-50',
              selectedRowKeys.includes(String(row.key)) ? 'bg-blue-50' : '',
            ]"
          >
            <td
              v-for="col in props.columns"
              :key="col.dataIndex"
              :class="['py-2', col.type !== 'check' ? 'px-4 ' : 'pl-11']"
              @dblclick="selectedItemUpdate(String(row.key), col.type)"
            >
              <template v-if="col.type === 'check'">
                <input
                  type="checkbox"
                  :checked="selectedRowKeys.includes(String(row.key))"
                  @change="() => toggleRowSelection(String(row.key))"
                />
              </template>

              <template v-else-if="col.type === 'label'">
                {{ row[col.dataIndex] }}
              </template>
              <template v-else-if="col.type === 'date'">
                {{ formatDate(row[col.dataIndex]) }}
              </template>
              <template v-else-if="col.type === 'link'">
                <router-link :to="{ name: 'customer' }" class="">
                  {{ row[col.dataIndex] }}
                </router-link>
              </template>
              <template v-else-if="col.type === 'custom'">
                <slot name="customType" :row="row" :col="col"></slot>
              </template>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Footer Table -->
    <div
      class="table-footer flex justify-between items-center px-2 py-2 border-t border-gray-200 bg-white"
    >
      <div class="flex items-center gap-5">
        <span class="icon icon-setting-pagination"> </span>
        <span class="font-14">
          Tổng số: <br />
          <span class="font-bold font-14">{{ props.toltalItem }}</span>
        </span>
        <span class="font-14">
          Công nợ: <br />
          <span class="font-bold font-14">0</span>
        </span>
      </div>
      <div class="flex items-center">
        <SelectedButton
          class="mx-2"
          :label-name="String(pageSizeOptions[0]?.name)"
          :options="pageSizeOptions"
          position="bottom-10"
          @seleted="handleChangePageSize"
          default-width="w-[148px]"
        />
        <span
          :class="[
            'icon icon-previous-start cursor-pointer',
            current === 1 ? 'opacity-30' : '',
          ]"
          @click="fistPage"
        >
        </span>
        <span
          @click="prevPage"
          :disabled="current === 1"
          :class="[
            'icon icon-previous cursor-pointer',
            current === 1 ? 'opacity-30' : '',
          ]"
        >
        </span>
        <span class="font-bold font-14">{{
          props.dataSource.length > 0 ? startRecord : 0
        }}</span>
        <span class="mx-2 font-14">đến</span>
        <span class="font-bold font-14">{{ endRecord }}</span>
        <span
          @click="nextPage"
          :disabled="current === totalPages"
          :class="[
            'icon icon-next cursor-pointer',
            current === totalPages ? 'opacity-30' : '',
          ]"
        >
        </span>
        <span
          :class="[
            'icon icon-next-last cursor-pointer',
            current === totalPages ? 'opacity-30' : '',
          ]"
          @click="lastPage"
        >
        </span>
      </div>
    </div>
  </div>
</template>
<style scoped>
th:hover .icon {
  opacity: 1;
}
</style>
