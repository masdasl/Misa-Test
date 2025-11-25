using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misa.Core.CoreAttribute
{
    [AttributeUsage(AttributeTargets.Property)]
    public class RequiredAttribute : Attribute
    {
        public RequiredAttribute(string error)
        {

        }
    }
}
