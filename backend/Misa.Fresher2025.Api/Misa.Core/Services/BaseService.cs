using Misa.Core.Interfaces.Repository;
using Misa.Core.Interfaces.Service;
using Misa.Core.Model;
using System;
using System.Collections.Generic;
using Misa.Core.DTo;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using Microsoft.AspNetCore.Http;


namespace Misa.Core.Services
{
    public class BaseService<TEntity, TKey, TKeys, TDto> : IBaseService<TEntity, TKey, TKeys, TDto>
    {
        private readonly IBaseRepo<TEntity, TKey, TKeys, TDto> _repository;
        public BaseService(IBaseRepo<TEntity, TKey, TKeys, TDto> repository) { _repository = repository; }

        // -------------------------------------------------------
        // Công dụng: Lấy dữ liệu khách hàng theo khóa chính và
        //            trả về dưới dạng DTO. Nếu không tìm thấy, trả về lỗi.
        // Đầu vào:
        //      - id: Khóa chính của bản ghi cần lấy
        // Đầu ra:
        //      - ApiResponse<TDto> chứa dữ liệu DTO hoặc thông báo lỗi
        // Ai tạo: Phan Duy Anh
        // Tạo ngày: 11/21/2025
        // -------------------------------------------------------
        public virtual ApiResponse<TDto> GetById(TKey id)
        {
            try
            {
                var entities = _repository.GetById(id);
                var dto = MapToDto(entities);
                if (dto == null)
                {
                    return new ApiResponse<TDto>
                    {
                        Error = "Không tìm thấy khách hàng"
                    };
                }
                return new ApiResponse<TDto>
                {
                    Data = dto,
                    Error = null
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<TDto>
                {
                    Error = ex.Message,
                };
            }
        }

        // -------------------------------------------------------
        // Công dụng: Thêm mới một hoặc nhiều bản ghi khách hàng
        //            vào database từ DTO.
        // Đầu vào:
        //      - entity: Mảng DTO chứa dữ liệu khách hàng cần thêm
        // Đầu ra:
        //      - ApiResponse<string>: "OK" nếu thành công, "Faile" + thông báo lỗi nếu thất bại
        // Ai tạo: Phan Duy Anh
        // Tạo ngày: 11/21/2025
        // -------------------------------------------------------
        public virtual ApiResponse<string> Insert(TDto[] entity)
        {
            try
            {
                var entities = _repository.Insert(entity);
                return new ApiResponse<string>
                {
                    Data = "OK",
                    Error = null
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>
                {
                    Data = "Faile",
                    Error = ex.Message,
                };
            }
        }

        // -------------------------------------------------------
        // Công dụng: Cập nhật dữ liệu khách hàng theo khóa chính.
        // Đầu vào:
        //      - id: Khóa chính của bản ghi cần cập nhật
        //      - entity: DTO chứa dữ liệu mới
        // Đầu ra:
        //      - ApiResponse<string>: "OK" nếu cập nhật thành công, "Faile" + thông báo lỗi nếu thất bại
        // Ai tạo: Phan Duy Anh
        // Tạo ngày: 11/21/2025
        // -------------------------------------------------------
        public ApiResponse<string> Update(TKey id, TDto entity)
        {
            try
            {
                var entities = _repository.Update(id, entity);
                return new ApiResponse<string>
                {
                    Data = "OK",
                    Error = null
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>
                {
                    Data = "Faile",
                    Error = ex.Message,
                };
            }
        }

        // -------------------------------------------------------
        // Công dụng: Xóa mềm (soft delete) khách hàng theo danh sách khóa chính.
        // Đầu vào:
        //      - id: Danh sách khóa chính cần xóa
        // Đầu ra:
        //      - ApiResponse<string>: "OK" nếu xóa thành công, "Faile" + thông báo lỗi nếu thất bại
        // Ai tạo: Phan Duy Anh
        // Tạo ngày: 11/21/2025
        // -------------------------------------------------------
        public virtual ApiResponse<string> Delete(TKeys id)
        {
            try
            {
                var entities = _repository.Delete(id);
                return new ApiResponse<string>
                {
                    Data = "OK",
                    Error = null
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>
                {
                    Data = "Faile",
                    Error = ex.Message,
                };
            }
        }

        // -------------------------------------------------------
        // Công dụng: Kiểm tra định dạng email hợp lệ.
        // Đầu vào:
        //      - email: Chuỗi email cần kiểm tra
        // Đầu ra:
        //      - bool: true nếu email hợp lệ, false nếu không hợp lệ
        // Ai tạo: Phan Duy Anh
        // Tạo ngày: 11/21/2025
        // -------------------------------------------------------
        public bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            var regex = new System.Text.RegularExpressions.Regex(
                @"^[^\s@]+@[^\s@]+\.[^\s@]+$",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase
            );

            return regex.IsMatch(email);
        }

        // -------------------------------------------------------
        // Công dụng: Kiểm tra định dạng số điện thoại hợp lệ.
        //            Chuyển số điện thoại thành dạng chỉ chứa chữ số,
        //            thêm số 0 nếu cần, kiểm tra độ dài 10-11 chữ số.
        // Đầu vào:
        //      - phone: Chuỗi số điện thoại cần kiểm tra
        // Đầu ra:
        //      - bool: true nếu hợp lệ, false nếu không hợp lệ
        // Ai tạo: Phan Duy Anh
        // Tạo ngày: 11/21/2025
        // -------------------------------------------------------
        public bool IsValidPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return false;
            phone = new string(phone.Where(char.IsDigit).ToArray());
            if (phone.Length == 9)
                phone = "0" + phone;

            var regex = new System.Text.RegularExpressions.Regex(@"^\d{10,11}$");
            return regex.IsMatch(phone);
        }

        // -------------------------------------------------------
        // Công dụng: Lấy danh sách khách hàng dạng bảng theo phân trang,
        //            lọc và sắp xếp, trả về kết quả dưới dạng DTO.
        // Đầu vào:
        //      - dataTableRequest: Thông tin filter, search, sort, paging
        // Đầu ra:
        //      - ApiResponse<IEnumerable<TDto>> chứa dữ liệu khách hàng
        //        và thông tin phân trang (Meta)
        // Ai tạo: Phan Duy Anh
        // Tạo ngày: 11/21/2025
        // -------------------------------------------------------
        public ApiResponse<IEnumerable<TDto>> LoadCustomersTable(DataTableRequest dataTableRequest)
        {
            (IEnumerable<TEntity> data, int Total) = _repository.LoadCustomersTable(dataTableRequest);
            var tableValue = data?.Select(entity => MapToDto(new List<TEntity> { entity })).Where(d => d != null).ToList();
            return new ApiResponse<IEnumerable<TDto>>
            {
                Data = tableValue,
                Meta = new ApiResponse<IEnumerable<TDto>>.Pagination
                {
                    PageSize = dataTableRequest.PageSize,
                    Page = dataTableRequest.PageNo,
                    Total = Total
                },
                Error = null
            };
        }

        // -------------------------------------------------------
        // Công dụng: Upload ảnh lên server, lưu vào thư mục wwwroot/uploads,
        //            trả về đường dẫn file (cast sang TKey).
        // Đầu vào:
        //      - image: File ảnh cần upload
        // Đầu ra:
        //      - ApiResponse<TKey>:
        //            + Data: đường dẫn file (TKey) nếu upload thành công
        //            + Error: thông báo lỗi nếu thất bại
        // Ai tạo: Phan Duy Anh
        // Tạo ngày: 11/21/2025
        // -------------------------------------------------------
        public ApiResponse<TKey> UploadImg(IFormFile image)
        {
            try
            {
                if (image == null || image.Length == 0)
                {
                    return new ApiResponse<TKey>
                    {
                        Data = default,
                        Error = "Ảnh không hợp lệ"
                    };
                }

                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                var filePath = Path.Combine(folderPath, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    image.CopyTo(stream);
                }
                string returnPath = $"/uploads/{fileName}";
                TKey castedPath = (TKey)(object)returnPath;

                return new ApiResponse<TKey>
                {
                    Data = castedPath,
                    Error = null
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<TKey>
                {
                    Data = default,
                    Error = ex.Message
                };
            }
        }

        // -------------------------------------------------------
        // Công dụng: Chuyển đổi entity sang DTO.
        //            Hàm được khai báo virtual, cần override trong lớp con.
        // Đầu vào:
        //      - entity: IEnumerable<TEntity cần map
        // Đầu ra:
        //      - TDto tương ứng với entity đầu tiên hoặc null
        // Ai tạo: Phan Duy Anh
        // Tạo ngày: 11/21/2025
        // -------------------------------------------------------
        protected virtual TDto MapToDto(IEnumerable<TEntity> entity) => throw new NotImplementedException();
    }
}
