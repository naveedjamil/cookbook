using Recipe.Core.Base.Generic;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using Test.Data.DTO;
using Test.Data.Model;
using Test.Service.Interface;

namespace Test.API.Controller
{
    [RoutePrefix("api/v1/Users")]
    [AllowAnonymous]
    public class TestController : Controller<IUserService, UserDTO, User, int>
    {
        public TestController(IUserService service) : base(service)
        {

        }

        //[HttpGet]
        //[Route("")]
        //public string Get()
        //{
        //    return "naveed";
        //}

        //public override async Task<UserDTO> Get(int id)
        //{
        //    return await base.Get(id);
        //}


        [HttpGet]
        [Route("")]
        public override async Task<IList<UserDTO>> Get()
        {
            return await base.Get();
        }

        //public override async Task<UserDTO> Post(UserDTO dtoObject)
        //{
        //    return await base.Post(dtoObject);
        //}

        //public override async Task<UserDTO> Put(UserDTO dtoObject)
        //{
        //    return await base.Put(dtoObject);
        //}

        //public override async Task Delete(int id)
        //{
        //    await base.Delete(id);
        //}
    }
}