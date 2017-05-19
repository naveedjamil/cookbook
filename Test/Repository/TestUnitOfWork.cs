using Recipe.Core.Base.Interface;
using Test.Data.Model;
using Test.Repository.Interface;
using Recipe.Core.Base.Abstract;
namespace Test.Repository
{
    public class TestUnitOfWork : UnitOfWork, ITestUnitOfWork
    {

        public TestUnitOfWork(
            IRequestInfo requestInfo,
            IUserRepository userRepository
            ) : base(requestInfo)
        {
            _userRepository = userRepository;
        }

        private readonly IUserRepository _userRepository;        

        public IUserRepository UserRepository
        {
            get
            {
                return _userRepository;
            }
        }

    }
}
