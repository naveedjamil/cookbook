using Recipe.Core.Base.Interface;
using Test.Data.DTO;
using Test.Data.Model;

namespace Test.Service.Interface
{
    public interface IUserService : IService<IRepository<User, int>, User, UserDTO, int>
    {
    }
}
