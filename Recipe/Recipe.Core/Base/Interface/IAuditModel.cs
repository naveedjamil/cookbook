using System;

namespace Recipe.Core.Base.Interface
{
    public interface IAuditModel<TKey> : IAuditModel, IBase<TKey>
    {
    }

    public interface IAuditModel : IBase
    {
        string CreatedBy { get; set; }
        DateTime CreatedOn { get; set; }
        string LastModifiedBy { get; set; }
        DateTime LastModifiedOn { get; set; }
        bool IsDeleted { get; set; }
    }
}
