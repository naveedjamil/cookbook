using Recipe.Common.Helper;
using Recipe.Core.Attribute;
using Recipe.Core.Enum;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Recipe.Core.Base.Interface
{
    public interface IRepository
    {
    }
    public interface IRepository<TEntity, TKey> : IRepository
    {
        [AuditOperation(OperationType.Read)]
        Task<TEntity> GetAsync(TKey id);

        [AuditOperation(OperationType.Read)]
        Task<IEnumerable<TEntity>> GetAsync(IList<TKey> ids);

        [AuditOperation(OperationType.Read)]
        Task<TEntity> GetEntityOnly(TKey id);

        [AuditOperation(OperationType.Read)]
        Task<int> GetCount();

        [AuditOperation(OperationType.Read)]
        Task<IEnumerable<TEntity>> GetAll();

        [AuditOperation(OperationType.Read)]
        Task<IEnumerable<TEntity>> GetAll(JsonApiRequest request);

        [AuditOperation(OperationType.Create)]
        Task<TEntity> Create(TEntity entity);

        [AuditOperation(OperationType.Update)]
        Task<TEntity> Update(TEntity entity);

        [AuditOperation(OperationType.Delete)]
        Task DeleteAsync(TKey id);
    }
}
