using Microsoft.Owin;
using Microsoft.Practices.Unity;
using Owin;
using Recipe.Core;
using Recipe.Core.Base.Interface;
using Test.API.Infrastructure;
using Test.Repository;
using Test.Service.Concrete;
using Test.Service.Interface;

[assembly: OwinStartup(typeof(Test.API.Startup))]
namespace Test.API
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            IoC.Container.RegisterType<IRequestInfo, RequestInfo>()
                .RegisterType<IUserService, UserService>()
                .RegisterType<IRepository<Test.Data.Model.User, int>, Recipe.Core.Base.Generic.Repository<Test.Data.Model.User, int>>()
                .RegisterType<ITestUnitOfWork, TestUnitOfWork>();
        }
    }
}