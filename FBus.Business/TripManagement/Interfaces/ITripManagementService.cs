using FBus.Business.TripManagement.SearchModel;
using FBus.Business.BaseBusiness.CommonModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBus.Business.TripManagement.Interfaces
{
    public interface ITripManagementService
    {
        Task<Response> Create(TripSearchModel model);
        Task<Response> Get(Guid id);
        Task<Response> GetList();
        Task<Response> Update(TripUpdateModel model, Guid id);
        Task<Response> Delete(Guid id);
    }
}
