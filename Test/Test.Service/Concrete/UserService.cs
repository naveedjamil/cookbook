using Recipe.Core.Base.Generic;
using Recipe.Core.Base.Interface;
using Test.Data.DTO;
using Test.Data.Model;
using Test.Repository;
using Test.Service.Interface;

namespace Test.Service.Concrete
{
    public class UserService : Service<IRepository<User, int>, User, UserDTO, int>, IUserService
    {
        public UserService(ITestUnitOfWork unitOfWork) : base(unitOfWork, unitOfWork.UserRepository)
        {

        }
    }
}
