using FBus.Business.RouteManagement.SearchModel;
using FBus.Business.BaseBusiness.CommonModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBus.Business.RouteManagement.Interfaces
{
    public interface IRouteManagementService
    {
        Task<Response> Create(RouteSearchModel model);
        Task<Response> Get(Guid id);
        Task<Response> GetList();
        //Task<Response> Update(RouteUpdateModel model, Guid id);
        Task<Response> Delete(Guid id);
    }
}
