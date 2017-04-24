using Recipe.Common.Helper;
using Recipe.Core.Base.Abstract;
using Recipe.Core.Base.Interface;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;

namespace Recipe.Core.Base.Generic
{
    [Authorize]
    [Route("")]
    public class Controller : ApiController
    {
        public async virtual Task<ResponseMessageResult> GetResponseMessage(HttpResponseMessage message)
        {
            return this.ResponseMessage(message);
        }
    }

    public abstract class Controller<TService, TDTO, TEntity, TKey> : Controller
     where TEntity : IAuditModel<TKey>, new()
     where TDTO : DTO<TEntity, TKey>, new()
     where TService : IService<TDTO, TKey>
    {
        TService _service;

        protected TService Service
        {
            get
            {
                return _service;
            }
        }

        public Controller(TService service)
        {
            this._service = service;
        }

        [HttpGet]
        [Route("")]
        public virtual Task<IList<TDTO>> Get()
        {
            var request = Request.GetJsonApiRequest();
            return this._service.GetAllAsync(request);
        }

        [HttpGet]
        [Route("{id}")]
        public virtual Task<TDTO> Get(TKey id)
        {
            return this._service.GetAsync(id);
        }

        [HttpPost]
        [Route("")]
        public virtual Task<TDTO> Post(TDTO dtoObject)
        {
            return this._service.CreateAsync(dtoObject);
        }

        [HttpPut]
        [Route("")]
        public virtual Task<TDTO> Put(TDTO dtoObject)
        {
            return this._service.UpdateAsync(dtoObject);
        }

        [HttpDelete]
        [Route("{id}")]
        public virtual Task Delete(TKey id)
        {
            return this._service.DeleteAsync(id);
        }
    }

}
