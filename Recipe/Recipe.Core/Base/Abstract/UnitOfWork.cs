using Recipe.Core.Base.Interface;
using System.Data.Entity;
using System.Threading.Tasks;

namespace Recipe.Core.Base.Abstract
{
    public abstract class UnitOfWork : IUnitOfWork
    {
        IRequestInfo _requestInfo;
        public UnitOfWork(IRequestInfo requestInfo)
        {
            _requestInfo = requestInfo;
        }
        public DbContext DBContext
        {
            get
            {
                return _requestInfo.Context;
            }
        }

        public int Save()
        {
            return _requestInfo.Context.SaveChanges();
        }
        public async Task<int> SaveAsync()
        {
            return await _requestInfo.Context.SaveChangesAsync();
        }
    }

}
