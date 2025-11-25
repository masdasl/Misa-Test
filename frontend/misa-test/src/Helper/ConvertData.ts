// -------------------------------------------------------
// Công dụng: chuyên đổi date sang format dd/mm/yyyy
// Đầu vào: value: string (là ngày giá trị ngày muốn chuyển đổi)
// Ai tạo: Phan Duy Anh
// Tạo ngày: 11/18/2025
// -------------------------------------------------------
export function formatDate(dateString: string): string {
  if (!dateString) return "";
  const date = new Date(dateString);
  if (isNaN(date.getTime())) return "";
  const day = String(date.getDate()).padStart(2, "0");
  const month = String(date.getMonth() + 1).padStart(2, "0");
  const year = date.getFullYear();
  return `${day}/${month}/${year}`;
}
