using Recipe.Core.Base.Interface;
using Test.Data.Model;

namespace Test.Repository
{
    public interface ITestUnitOfWork : IUnitOfWork
    {
        IRepository<User, int> UserRepository { get; }
    }
}
