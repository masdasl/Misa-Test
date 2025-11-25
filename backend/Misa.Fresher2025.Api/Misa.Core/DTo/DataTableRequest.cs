using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misa.Core.DTo
{
    public class DataTableRequest
    {
        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public string SortKey { get; set; }
        public string SortType { get; set; }
        public Dictionary<string, string>? Filters { get; set; }
        public Dictionary<string, string>? SearchValue { get; set; }
    }
}
