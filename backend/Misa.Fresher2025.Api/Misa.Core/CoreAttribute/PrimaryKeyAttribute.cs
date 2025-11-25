using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misa.Core.CoreAttribute
{
    [AttributeUsage(AttributeTargets.Property)]
    public class PrimaryKeyAttribute : Attribute
    {
        public string ColumnName { get; set; }

        public PrimaryKeyAttribute(string columnName)
        {
            ColumnName = columnName;
        }
    }
}
