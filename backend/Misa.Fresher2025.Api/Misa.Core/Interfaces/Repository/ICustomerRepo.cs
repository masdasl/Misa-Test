using Misa.Core.DTo;
using Misa.Core.Entities;
using Misa.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misa.Core.Interfaces.Repository
{
    public interface ICustomerRepo : IBaseRepo<Customer, string, string[], CustomerDto>
    {
        string createNumber(string prefix);
        int GetMaxNumberByPrefix(string prefix);
    }
}
