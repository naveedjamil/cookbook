using Recipe.Core.Base.Interface;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Recipe.Core.Base.Generic
{
    public class AuditableRepository<TEntity, TKey> : Repository<TEntity, TKey>
            where TEntity : class, IAuditModel<TKey>
            where TKey : IEquatable<TKey>
    {
        public AuditableRepository(IRequestInfo requestInfo)
            : base(requestInfo)
        {
        }

        protected override IQueryable<TEntity> DefaultListQuery
        {
            get
            {
                return base.DefaultListQuery.Where(x => !x.IsDeleted);
            }
        }

        protected override IQueryable<TEntity> DefaultSingleQuery
        {
            get
            {
                return base.DefaultSingleQuery.Where(x => !x.IsDeleted);
            }
        }

        public override async Task<TEntity> Create(TEntity entity)
        {
            entity.CreatedBy = RequestInfo.UserId;
            entity.CreatedOn = DateTime.UtcNow;
            entity.LastModifiedBy = RequestInfo.UserId;
            entity.LastModifiedOn = DateTime.UtcNow;
            entity.IsDeleted = false;
            return await base.Create(entity);
        }

        public override async Task<TEntity> Update(TEntity entity)
        {
            entity.LastModifiedOn = DateTime.UtcNow;
            entity.LastModifiedBy = RequestInfo.UserId;
            return await base.Update(entity);
        }

        public override async Task DeleteAsync(TKey id)
        {
            var entity = await GetAsync(id);
            if (entity != null)
            {
                entity.LastModifiedOn = DateTime.UtcNow;
                entity.LastModifiedBy = RequestInfo.UserId;
                entity.IsDeleted = true;
                await base.Update(entity);
            }
        }

        protected void UpdateChildrenWithoutLog<TChildEntity>(ICollection<TChildEntity> childEntities) where TChildEntity : class, IBase<int>
        {
            foreach (var entity in childEntities)
            {
                UpdateChildrenWithOutLog(entity);
            }
        }

        public virtual void UpdateChildrenWithOutLog<TChildEntity>(TChildEntity childEntity) where TChildEntity : class, IBase<int>
        {
            if (childEntity.Id > 0)
            {
                DBContext.Entry(childEntity).State = EntityState.Modified;
            }
            else
            {
                DBContext.Entry(childEntity).State = EntityState.Added;
            }
        }
    }
}
