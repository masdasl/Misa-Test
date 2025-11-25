using Misa.Core.DTo;
using Misa.Core.Entities;
using Misa.Core.Interfaces.Repository;
using Misa.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misa.Core.Interfaces.Service
{
    public interface ICustomerService : IBaseService<Customer, string, string[], CustomerDto>
    {
        ApiResponse<string> CreateCustomerCode();
        ApiResponse<string> CheckEmail(string email);
        ApiResponse<string> CheckPhone(string phone);
        ApiResponse<List<Dictionary<string, string>>> ImportCustomers(Stream excelFile);
    }
}
