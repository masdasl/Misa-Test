import { useToast } from "vue-toast-notification";
import { ToastBase } from "../constant/toastBase";

const toast = useToast();

// -------------------------------------------------------
// Công dụng: Hiển thị thông báo lỗi dạng toast
// Đầu vào: text: string - Nội dung thông báo lỗi
// Ai tạo: Phan Duy Anh
// Tạo ngày: 11/18/2025
// -------------------------------------------------------
export function ErrorNotification(text: string) {
  return toast.error(text, ToastBase);
}

// -------------------------------------------------------
// Công dụng: Hiển thị thông báo thành công dạng toast
// Đầu vào: text: string - Nội dung thông báo thành công
// Ai tạo: Phan Duy Anh
// Tạo ngày: 11/18/2025
// -------------------------------------------------------
export function SuccessNotification(text: string) {
  return toast.success(text, ToastBase);
}

// -------------------------------------------------------
// Công dụng: Hiển thị cảnh báo dạng toast
// Đầu vào: text: string - Nội dung cảnh báo
// Ai tạo: Phan Duy Anh
// Tạo ngày: 11/18/2025
// -------------------------------------------------------
export function WarningNotification(text: string) {
  return toast.warning(text, ToastBase);
}

// -------------------------------------------------------
// Công dụng: Hiển thị thông báo thông tin dạng toast
// Đầu vào: text: string - Nội dung thông báo thông tin
// Ai tạo: Phan Duy Anh
// Tạo ngày: 11/18/2025
// -------------------------------------------------------
export function InfoNotification(text: string) {
  return toast.info(text, ToastBase);
}
