using Misa.Core.DTo;
using Misa.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;


namespace Misa.Core.Interfaces.Service
{
    public interface IBaseService<TEntity, TKey, TKeys, TDto>
    {
        ApiResponse<TDto> GetById(TKey id);
        ApiResponse<string> Insert(TDto[] entity);
        ApiResponse<string> Update(TKey id, TDto entity);
        ApiResponse<string> Delete(TKeys id);
        ApiResponse<IEnumerable<TDto>> LoadCustomersTable(DataTableRequest dataTableRequest);
        ApiResponse<TKey> UploadImg(IFormFile image);
        bool IsValidEmail(string email);

        bool IsValidPhone(string phone);
    }
}
