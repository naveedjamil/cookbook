using System.Data.Entity;
using System.Threading.Tasks;

namespace Recipe.Core.Base.Interface
{
    public interface IUnitOfWork
    {
        DbContext DBContext { get; }
        Task<int> SaveAsync();
        int Save();
    }
}
