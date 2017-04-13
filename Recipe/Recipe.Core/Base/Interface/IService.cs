using Recipe.Common.Helper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Recipe.Core.Base.Interface
{
    public interface IService
    {
        IUnitOfWork UnitOfWork { get; }
    }

    public interface IService<TDTO, TKey> : IService
    {
        Task<TDTO> GetAsync(TKey id);

        Task<int> GetCount();

        Task<IList<TDTO>> GetAllAsync();

        Task<IList<TDTO>> GetAllAsync(JsonApiRequest request);

        Task<TDTO> CreateAsync(TDTO dtoObject);

        Task<TDTO> UpdateAsync(TDTO dtoObject);

        Task DeleteAsync(TKey id);


        Task<IList<TDTO>> CreateAsync(IList<TDTO> dtoObjects);

        Task DeleteAsync(IList<TKey> ids);

        Task<IList<TDTO>> UpdateAsync(IList<TDTO> dtoObjects);
    }

    public interface IService<TRepository, TEntity, TDTO, TKey> : IService<TDTO, TKey>
    {
        TRepository Repository { get; }
    }
}

