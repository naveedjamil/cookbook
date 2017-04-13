using Recipe.Common.Helper;
using Recipe.Core.Base.Interface;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Recipe.Core.Base.Generic
{
    public class Repository<TEntity, TKey> : IRepository<TEntity, TKey>
        where TEntity : class, IBase<TKey>
        where TKey : IEquatable<TKey>
    {
        protected IRequestInfo RequestInfo { get; private set; }

        protected DbContext DBContext
        {
            get { return RequestInfo.Context; }
        }

        protected IDbSet<TEntity> DbSet
        {
            get
            {
                return DBContext.Set<TEntity>();
            }
        }

        protected virtual IQueryable<TEntity> DefaultListQuery
        {
            get
            {
                return DBContext.Set<TEntity>().AsQueryable().OrderBy(x => x.Id);
            }
        }

        protected virtual IQueryable<TEntity> DefaultSingleQuery
        {
            get
            {
                return DBContext.Set<TEntity>().AsQueryable();
            }
        }

        public Repository(IRequestInfo requestInfo)
        {
            RequestInfo = requestInfo;
        }

        public virtual async Task<TEntity> GetAsync(TKey id)
        {
            return await DefaultSingleQuery.SingleOrDefaultAsync(x => x.Id.Equals(id));
        }

        public virtual async Task<IEnumerable<TEntity>> GetAsync(IList<TKey> ids)
        {
            return await DefaultSingleQuery.Where(x => ids.Contains(x.Id)).ToListAsync();
        }

        public virtual async Task<int> GetCount()
        {
            return await this.DefaultListQuery.CountAsync();
        }

        public virtual async Task<IEnumerable<TEntity>> GetAll()
        {
            return await DefaultListQuery.ToListAsync();
        }

        public virtual async Task<IEnumerable<TEntity>> GetAll(JsonApiRequest request)
        {
            return await GetAllQueryable(request).ToListAsync();
        }

        protected virtual IQueryable<TEntity> GetAllQueryable(JsonApiRequest request)
        {
            return DefaultListQuery.GenerateQuery(request);
        }

        public virtual async Task<TEntity> Create(TEntity entity)
        {
            DBContext.Entry(entity).State = EntityState.Added;
            return entity;
        }

        public virtual async Task<TEntity> Update(TEntity entity)
        {
            var localEntity = GetExisting(entity);
            if (localEntity != null)
            {
                if (!RemoveExistingEntity(localEntity))
                {
                    throw new ApplicationException("Unexpected Error Occured");
                }
            }
            DBContext.Entry(entity).State = EntityState.Modified;
            return entity;
        }

        public virtual async Task DeleteAsync(TKey id)
        {
            var entity = await GetAsync(id);
            DBContext.Entry(entity).State = EntityState.Deleted;
        }

        protected void DeleteRange<TEntityList>(TEntityList entityList) where TEntityList : IQueryable
        {
            foreach (var each in entityList)
            {
                DBContext.Entry(each).State = EntityState.Deleted;
            }
        }

        public virtual async Task<TEntity> GetEntityOnly(TKey id)
        {
            return await DBContext.Set<TEntity>().AsQueryable().SingleOrDefaultAsync(x => x.Id.Equals(id));
        }

        private TEntity GetExisting(TEntity entity)
        {
            return DBContext.Set<TEntity>().Local.FirstOrDefault(x => x.Id.Equals(entity.Id));
        }

        private bool RemoveExistingEntity(TEntity entity)
        {
            return DBContext.Set<TEntity>().Local.Remove(entity);
        }

    }
}
