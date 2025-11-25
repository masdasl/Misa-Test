import Swal from "sweetalert2";

export function showConfirm(options: {
  title: string;
  message?: string;
  confirmText?: string;
  cancelText?: string;
  customerModel?: string;
  isHiddenCancelBtn?: boolean;
}) {
  const showCancel = !options.isHiddenCancelBtn;
  const cancelBtnText = options.cancelText || "Hủy bỏ";
  return Swal.fire({
    title: options.title,
    html: options.customerModel || `<p class="font-14">${options.message}</p>`,
    showCancelButton: showCancel,
    confirmButtonText: options.confirmText || "Đồng ý",
    cancelButtonText: cancelBtnText,

    buttonsStyling: false,
    customClass: {
      title: "font-18 font-bold text-black",
      htmlContainer: "font-14",

      confirmButton:
        "px-4 py-2 bg-blue-500 text-white rounded hover:bg-blue-600 cursor-pointer",
      cancelButton:
        "px-4 py-2 border border-blue-500 rounded bg-white text-primary cursor-pointer mx-3",
    },
  });
}
