using Recipe.Core.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Recipe.Core.Attribute
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class AuditOperation : System.Attribute
    {
        public AuditOperation(OperationType operationType)
        {
            OperationType = operationType;
        }

        public OperationType OperationType { get; set; }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class AuditOperationIgnore : System.Attribute
    {
        public AuditOperationIgnore()
        {
        }
    }
}