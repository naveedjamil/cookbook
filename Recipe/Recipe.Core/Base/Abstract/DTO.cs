using Recipe.Core.Base.Interface;
using System.Collections.Generic;
using System.Linq;

namespace Recipe.Core.Base.Abstract
{
    public class DTO<TEntity, TKey> : IBase<TKey>
        where TEntity : IAuditModel<TKey>, new()
    {
        public TKey Id { get; set; }

        public DTO()
        {

        }

        public DTO(TEntity entity)
        {
            ConvertFromEntity(entity);
        }

        public TEntity ConvertToEntity()
        {
            TEntity entity = new TEntity();
            return ConvertToEntity(entity);
        }

        public virtual TEntity ConvertToEntity(TEntity entity)
        {
            entity.Id = Id == null || Id.Equals(default(TKey)) ? entity.Id : Id;
            return entity;
        }

        public virtual void ConvertFromEntity(TEntity entity)
        {
            Id = entity.Id;
        }

        public static List<TDTO> ConvertEntityListToDTOList<TDTO>(IEnumerable<TEntity> entityList) where TDTO : DTO<TEntity, TKey>, new()
        {
            var result = new List<TDTO>();
            if (entityList != null)
                foreach (var entity in entityList)
                {
                    var dto = new TDTO();
                    dto.ConvertFromEntity(entity);
                    result.Add(dto);
                }
            return result;
        }

        public static IList<TEntity> ConvertDTOListToEntity(IEnumerable<DTO<TEntity, TKey>> dtoList, IEnumerable<TEntity> entityList)
        {
            var result = new List<TEntity>();
            if (dtoList != null)
                foreach (var dto in dtoList)
                {
                    var entityFromDb = entityList.SingleOrDefault(x => x.Id.Equals(dto.Id));
                    if (entityFromDb != null)
                    {
                        result.Add(dto.ConvertToEntity(entityFromDb));
                    }
                    else
                    {
                        result.Add(dto.ConvertToEntity());
                    }
                }
            foreach (var entity in entityList.Where(x => !dtoList.Any(y => y.Id.Equals(x.Id))))
            {
                entity.IsDeleted = true;
                result.Add(entity);
            }
            return result;
        }

        public static IList<TEntity> ConvertDTOListToEntity(IEnumerable<DTO<TEntity, TKey>> dtoList)
        {
            var result = new List<TEntity>();
            if (dtoList != null)
                foreach (var dto in dtoList)
                {
                    result.Add(dto.ConvertToEntity());
                }
            return result;
        }
    }
}
