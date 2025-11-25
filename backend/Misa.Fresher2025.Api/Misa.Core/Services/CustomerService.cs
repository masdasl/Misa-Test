using Misa.Core.DTo;
using Misa.Core.Entities;
using Misa.Core.Interfaces.Repository;
using Misa.Core.Interfaces.Service;
using Misa.Core.Model;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Misa.Core.Services
{
    public class CustomerService : BaseService<Customer, string, string[], CustomerDto>, ICustomerService
    {
        private readonly ICustomerRepo _repository;
        public CustomerService(ICustomerRepo repository) : base(repository)
        {
            _repository = repository;
        }

        // -------------------------------------------------------
        // Công dụng: Kiểm tra tính hợp lệ của email và kiểm tra xem
        //            email đã tồn tại trong hệ thống hay chưa.
        // Các bước thực hiện:
        //      1. Kiểm tra định dạng email
        //      2. Gọi repository để kiểm tra tồn tại trong database
        // Đầu vào:
        //      - email: Chuỗi email cần kiểm tra
        // Đầu ra:
        //      - ApiResponse<string>:
        //            + Data = "OK" nếu email hợp lệ và chưa tồn tại
        //            + Error = thông báo lỗi nếu email sai định dạng hoặc đã tồn tại
        // Ai tạo: Phan Duy Anh
        // Tạo ngày: 11/21/2025
        // -------------------------------------------------------
        public ApiResponse<string> CheckEmail(string email)
        {
            if (!IsValidEmail(email))
            {
                return new ApiResponse<string>
                {
                    Data = "Faile",
                    Error = "Email không đúng định dạng"
                };
            }
            string checkEmail = _repository.CheckValid("email", email);
            return new ApiResponse<string>
            {
                Data = checkEmail != null ? null : "OK",
                Error = checkEmail != null ? $"Email '{checkEmail}' đã tồn tại " : null
            };
        }

        // -------------------------------------------------------
        // Công dụng: Kiểm tra tính hợp lệ của số điện thoại và kiểm tra
        //            xem số điện thoại đã tồn tại trong hệ thống hay chưa.
        // Các bước thực hiện:
        //      1. Kiểm tra định dạng số điện thoại
        //      2. Gọi repository để kiểm tra tồn tại trong database
        // Đầu vào:
        //      - phone: Chuỗi số điện thoại cần kiểm tra
        // Đầu ra:
        //      - ApiResponse<string>:
        //            + Data = "OK" nếu số điện thoại hợp lệ và chưa tồn tại
        //            + Error = thông báo lỗi nếu định dạng sai hoặc đã tồn tại
        // Ai tạo: Phan Duy Anh
        // Tạo ngày: 11/21/2025
        // -------------------------------------------------------
        public ApiResponse<string> CheckPhone(string phone)
        {
            if (!IsValidPhone(phone))
            {
                return new ApiResponse<string>
                {
                    Data = "Faile",
                    Error = "Số điện thoại không đúng định dạng"
                };
            }
            string checkPhone = _repository.CheckValid("phone", phone);
            return new ApiResponse<string>
            {
                Data = checkPhone != null ? null : "OK",
                Error = checkPhone != null ? $"Số điện thoại '{checkPhone}' đã tồn tại " : null
            };
        }

        // -------------------------------------------------------
        // Công dụng: Tạo mã khách hàng mới theo format "KHyyyyMMxxxxxx"
        //            Trong đó:
        //              - "KH" là tiền tố cố định
        //              - "yyyyMM" là năm + tháng hiện tại
        //              - "xxxxxx" là số thứ tự tăng dần 6 chữ số
        // Đầu vào: Không có đầu vào
        // Đầu ra: ApiResponse<string> chứa mã khách hàng mới
        // Ai tạo: Phan Duy Anh
        // Tạo ngày: 11/21/2025
        // -------------------------------------------------------
        public ApiResponse<string> CreateCustomerCode()
        {
            var now = DateTime.Now;
            string prefix = $"KH{now:yyyyMM}";
            string nextNumber = _repository.createNumber(prefix);
            string data = string.Concat(prefix, nextNumber);
            var phoneSet = new HashSet<string>();
            var emailSet = new HashSet<string>();
            return new ApiResponse<string>
            {
                Data = data,
                Error = null
            };
        }

        // -------------------------------------------------------
        // Công dụng: Nhập dữ liệu khách hàng từ file Excel và lưu vào database.
        //            Quy trình thực hiện:
        //              1. Đọc file Excel (dòng 2 trở đi)
        //              2. Sinh mã khách hàng tự động theo prefix "KHyyyyMM" + số thứ tự
        //              3. Kiểm tra hợp lệ dữ liệu:
        //                   - Số điện thoại và email không để trống
        //                   - Không trùng số điện thoại/email trong file
        //                   - Không trùng số điện thoại/email trong database
        //              4. Lưu các bản ghi hợp lệ vào database
        //              5. Trả về danh sách lỗi nếu có
        // Đầu vào:
        //      - excelFile: Stream file Excel chứa dữ liệu khách hàng
        // Đầu ra:
        //      - ApiResponse<List<Dictionary<string,string>>>:
        //            + Data: danh sách các lỗi từng bản ghi (nếu có)
        //            + Error: "Fail" nếu có lỗi, null nếu tất cả bản ghi hợp lệ
        // Ai tạo: Phan Duy Anh
        // Tạo ngày: 11/21/2025
        // -------------------------------------------------------

        public ApiResponse<List<Dictionary<string, string>>> ImportCustomers(Stream excelFile)
        {

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var resultErrors = new List<Dictionary<string, string>>();
            var validCustomers = new List<CustomerDto>();
            var phoneSet = new HashSet<string>();
            var emailSet = new HashSet<string>();

            try
            {
                using var package = new ExcelPackage(excelFile);
                var sheet = package.Workbook.Worksheets.FirstOrDefault();
                if (sheet == null || sheet.Dimension == null)
                    return new ApiResponse<List<Dictionary<string, string>>> { Data = new List<Dictionary<string, string>>(), Error = "Fail" };

                int rowCount = sheet.Dimension.End.Row;
                var now = DateTime.Now;
                string prefix = $"KH{now:yyyyMM}";
                int maxNumber = _repository.GetMaxNumberByPrefix(prefix);
                for (int row = 2; row <= rowCount; row++)
                {

                    var customer = new CustomerDto
                    {
                        CustomerCode = prefix + (maxNumber + 1).ToString("D6"),
                        FullName = sheet.Cells[row, 1].Text.Trim(),
                        Phone = sheet.Cells[row, 2].Text.Trim(),
                        Email = sheet.Cells[row, 3].Text.Trim(),
                        Address = sheet.Cells[row, 4].Text.Trim(),
                        CustomerType = sheet.Cells[row, 5].Text.Trim(),
                        Zalo = sheet.Cells[row, 6].Text.Trim(),
                        TaxCode = sheet.Cells[row, 7].Text.Trim(),
                        ShippingAddress = sheet.Cells[row, 8].Text.Trim(),
                        BillingAddress = sheet.Cells[row, 9].Text.Trim(),
                        LastPurchaseDate = DateTime.TryParse(sheet.Cells[row, 10].Text.Trim(), out var date) ? date : (DateTime?)null,
                        PurchasedItems = sheet.Cells[row, 11].Text.Trim(),
                        LastPurchasedItem = sheet.Cells[row, 12].Text.Trim(),
                    };
                    maxNumber++;
                    string phoneError = "";
                    string emailError = "";
                    if (string.IsNullOrWhiteSpace(customer.Phone))
                        phoneError = "Số điện thoại không được để trống";
                    if (string.IsNullOrWhiteSpace(customer.Email))
                        emailError = "Email không được để trống";

                    if (!phoneSet.Add(customer.Phone))
                        phoneError = $"Số điện thoại 0{customer.Phone} đã tồn tại trong file";
                    if (!emailSet.Add(customer.Email))
                        emailError = $"Email {customer.Email} đã tồn tại trong file";

                    var phoneCheck = CheckPhone(customer.Phone);
                    if (phoneCheck.Error != null)
                        phoneError = phoneCheck.Error;

                    var emailCheck = CheckEmail(customer.Email);
                    if (emailCheck.Error != null)
                        emailError = emailCheck.Error;

                    if (!string.IsNullOrEmpty(phoneError) || !string.IsNullOrEmpty(emailError))
                    {
                        resultErrors.Add(new Dictionary<string, string>
                        {
                            ["full_name"] = customer.FullName,
                            ["phone"] = phoneError,
                            ["email"] = emailError
                        });
                        continue;
                    }
                    validCustomers.Add(customer);
                }

                if (validCustomers.Any())
                {
                    var insertResult = _repository.Insert(validCustomers.ToArray());
                    if (!insertResult.Equals("OK", StringComparison.OrdinalIgnoreCase))
                        resultErrors.Add(new Dictionary<string, string> { { "insert", insertResult } });
                }

                return new ApiResponse<List<Dictionary<string, string>>>
                {
                    Data = resultErrors,
                    Error = resultErrors.Any() ? "Fail" : null
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<Dictionary<string, string>>>
                {
                    Data = new List<Dictionary<string, string>> { new Dictionary<string, string> { { "exception", ex.Message } } },
                    Error = "Fail"
                };
            }
        }

        // -------------------------------------------------------
        // Công dụng: Chuyển đổi entity Customer sang DTO CustomerDto.
        //            Chỉ lấy bản ghi đầu tiên trong danh sách entities.
        // Đầu vào:
        //      - entities: IEnumerable<Customer> cần map
        // Đầu ra:
        //      - CustomerDto tương ứng với entity đầu tiên hoặc null nếu danh sách rỗng
        // Ai tạo: Phan Duy Anh
        // Tạo ngày: 11/21/2025
        // -------------------------------------------------------
        protected override CustomerDto MapToDto(IEnumerable<Customer> entities)
        {
            var entity = entities?.FirstOrDefault();
            if (entity == null) return null;

            return new CustomerDto
            {
                CustomerId = entity.CustomerId,
                CustomerCode = entity.CustomerCode,
                FullName = entity.FullName,
                Phone = entity.Phone,
                Email = entity.Email,
                Zalo = entity.Zalo,
                Address = entity.Address,
                TaxCode = entity.TaxCode,
                CustomerType = entity.CustomerType,
                ShippingAddress = entity.ShippingAddress,
                BillingAddress = entity.BillingAddress,
                LastPurchaseDate = entity.LastPurchaseDate,
                PurchasedItems = entity.PurchasedItems,
                LastPurchasedItem = entity.LastPurchasedItem,
                Avatar = entity.Avatar,
            };
        }
    }

}
