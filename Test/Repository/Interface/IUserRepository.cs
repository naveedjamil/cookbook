using Recipe.Core.Base.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test.Data.Model;

namespace Test.Repository.Interface
{
    public interface IUserRepository : IRepository<User, int>
    {
    }
}
