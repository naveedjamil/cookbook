using Recipe.Common.Helper;
using Recipe.Core.Base.Abstract;
using Recipe.Core.Base.Interface;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Recipe.Core.Base.Generic
{
    public class Service : IService
    {
        public IUnitOfWork UnitOfWork { get; private set; }

        public Service(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }
    }

    public class Service<TRepository, TEntity, TDTO, TKey> : Service, IService<TRepository, TEntity, TDTO, TKey>
     where TEntity : IAuditModel<TKey>, new()
     where TDTO : DTO<TEntity, TKey>, new()
     where TRepository : IRepository<TEntity, TKey>
     where TKey : IEquatable<TKey>
    {
        private TRepository _repository;

        public TRepository Repository
        {
            get
            {
                return _repository;
            }
        }

        public Service(IUnitOfWork unitOfWork, TRepository repository)
            : base(unitOfWork)
        {
            this._repository = repository;
        }

        protected async Task<TEntity> Create(TDTO dtoObject)
        {
            TEntity entity = dtoObject.ConvertToEntity();
            return await this._repository.Create(entity);
        }

        public async Task<TDTO> CreateAsync(TDTO dtoObject)
        {
            var result = await this.Create(dtoObject);
            await this.UnitOfWork.SaveAsync();

            dtoObject.ConvertFromEntity(result);
            return dtoObject;
        }

        public async Task<IList<TDTO>> CreateAsync(IList<TDTO> dtoObjects)
        {
            List<TEntity> results = new List<TEntity>();
            foreach (TDTO dtoObject in dtoObjects)
            {
                results.Add(await this.Create(dtoObject));
            }

            this.UnitOfWork.DBContext.Configuration.AutoDetectChangesEnabled = false;
            await this.UnitOfWork.SaveAsync();
            this.UnitOfWork.DBContext.Configuration.AutoDetectChangesEnabled = true;

            return DTO<TEntity, TKey>.ConvertEntityListToDTOList<TDTO>(results);
        }

        protected async Task Delete(TKey id)
        {
            await this._repository.DeleteAsync(id);
        }

        public async Task DeleteAsync(TKey id)
        {
            await this.Delete(id);
            await this.UnitOfWork.SaveAsync();
        }

        public async Task DeleteAsync(IList<TKey> ids)
        {
            foreach (TKey id in ids)
            {
                await this.Delete(id);
            }

            this.UnitOfWork.DBContext.Configuration.AutoDetectChangesEnabled = false;
            await this.UnitOfWork.SaveAsync();
            this.UnitOfWork.DBContext.Configuration.AutoDetectChangesEnabled = true;
        }

        public async Task<int> GetCount()
        {
            return await this._repository.GetCount();
        }

        public async Task<IList<TDTO>> GetAllAsync()
        {
            IEnumerable<TEntity> entity = await this._repository.GetAll();
            return DTO<TEntity, TKey>.ConvertEntityListToDTOList<TDTO>(entity);
        }

        public async Task<IList<TDTO>> GetAllAsync(JsonApiRequest request)
        {
            IEnumerable<TEntity> entity = await this._repository.GetAll(request);
            return DTO<TEntity, TKey>.ConvertEntityListToDTOList<TDTO>(entity);
        }

        public async Task<TDTO> GetAsync(TKey id)
        {
            TEntity entity = await _repository.GetAsync(id);
            if (entity == null)
                return null;
            TDTO dto = new TDTO();
            dto.ConvertFromEntity(entity);
            return dto;
        }

        protected async Task<TEntity> Update(TDTO dtoObject)
        {
            var dbEntity = await _repository.GetAsync(dtoObject.Id);
            var entity = dtoObject.ConvertToEntity(dbEntity);
            return await _repository.Update(entity);
        }

        public async Task<TDTO> UpdateAsync(TDTO dtoObject)
        {
            var result = await this.Update(dtoObject);
            await UnitOfWork.SaveAsync();
            dtoObject.ConvertFromEntity(result);
            return dtoObject;
        }

        public async Task<IList<TDTO>> UpdateAsync(IList<TDTO> dtoObjects)
        {
            List<TEntity> results = new List<TEntity>();
            foreach (TDTO dtoObject in dtoObjects)
            {
                results.Add(await this.Update(dtoObject));
            }

            this.UnitOfWork.DBContext.Configuration.AutoDetectChangesEnabled = false;
            await this.UnitOfWork.SaveAsync();
            this.UnitOfWork.DBContext.Configuration.AutoDetectChangesEnabled = true;

            return DTO<TEntity, TKey>.ConvertEntityListToDTOList<TDTO>(results);
        }

        public async Task<IList<TEntity>> UpdateAsync(IList<TEntity> entityObjects)
        {
            foreach (var entityObject in entityObjects)
            {
                await this._repository.Update(entityObject);
            }

            this.UnitOfWork.DBContext.Configuration.AutoDetectChangesEnabled = false;
            await this.UnitOfWork.SaveAsync();
            this.UnitOfWork.DBContext.Configuration.AutoDetectChangesEnabled = true;

            return entityObjects;
        }
    }
}
