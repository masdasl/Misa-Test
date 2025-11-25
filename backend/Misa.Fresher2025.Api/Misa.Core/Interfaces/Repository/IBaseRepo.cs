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
    public interface IBaseRepo<TEntity, TKey, TKeys, TDto>
    {
        IEnumerable<TEntity> GetById(TKey id);
        string Insert(TDto[] dtos);
        string Update(TKey id, TDto dto);
        string Delete(TKeys ids);
        TKey CheckValid(string columnName, TKey value);
        public (IEnumerable<TEntity> Data, int Total) LoadCustomersTable(DataTableRequest dataTableRequest);
    }

}
