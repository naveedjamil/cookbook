using Recipe.Core.Base.Interface;
using System;

namespace Recipe.Core.Base.Abstract
{
    public abstract class EntityBase<TKey> : IAuditModel<TKey>
    {
        public TKey Id { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime LastModifiedOn { get; set; }
        public bool IsDeleted { get; set; }
    }
}