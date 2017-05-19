using Recipe.Core.Base.Interface;
using Test.Data.Model;
using Test.Repository.Interface;

namespace Test.Repository
{
    public interface ITestUnitOfWork : IUnitOfWork
    {
        IUserRepository UserRepository { get; }
    }
}
