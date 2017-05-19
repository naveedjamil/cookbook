using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Recipe.Core.Base.Generic;
using Test.Data.Model;
using Test.Repository.Interface;
using Recipe.Core.Base.Interface;

namespace Test.Repository.Concrete
{
    public class UserRepository : Repository<User, int>, IUserRepository
    {
        
        public UserRepository(IRequestInfo requestInfo) : base(requestInfo)
        {

        }
    }
}
