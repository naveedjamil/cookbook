using Recipe.Core.Base.Interface;
using System;
using System.Data.Entity;
using System.Web;
using Test.Data.DbContext;

namespace Test.API.Infrastructure
{
    public class RequestInfo : IRequestInfo
    {
        private const string _applicationConfigContextKey = "ApplicationConfigContext";

        public RequestInfo()
        {
        }

        public string Role
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string UserId
        {
            get
            {
                return "Test";
            }
        }

        public string UserName
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        DbContext IRequestInfo.Context
        {
            get
            {
                ApplicationDbContext context;

                if (HttpContext.Current.Items.Contains(_applicationConfigContextKey))
                {
                    context = (ApplicationDbContext)HttpContext.Current.Items[_applicationConfigContextKey];
                }
                else
                {
                    context = new ApplicationDbContext();
                    HttpContext.Current.Items[_applicationConfigContextKey] = context;
                }

                return context;
            }
        }
    }
}