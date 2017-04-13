using Recipe.Core.Base.Interface;
using Test.Data.Model;

namespace Test.Repository
{
    public class TestUnitOfWork : Recipe.Core.Base.Abstract.UnitOfWork, ITestUnitOfWork
    {

        public TestUnitOfWork(
            IRequestInfo requestInfo,
            IRepository<User, int> userRepository
            ) : base(requestInfo)
        {
            _userRepository = userRepository;
        }

        private readonly IRepository<User, int> _userRepository;

        public IRepository<User, int> UserRepository
        {
            get
            {
                return _userRepository;
            }
        }
    }
}
