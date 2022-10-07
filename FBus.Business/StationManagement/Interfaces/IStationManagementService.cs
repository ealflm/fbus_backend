using FBus.Business.StationManagement.SearchModel;
using FBus.Business.BaseBusiness.CommonModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBus.Business.StationManagement.Interfaces
{
    public interface IStationManagementService
    {
        Task<Response> Create(StationSearchModel model);
        Task<Response> Get(Guid id);
        Task<Response> GetList();
        Task<Response> Update(StationUpdateModel model, Guid id);
        Task<Response> Delete(Guid id);
    }
}
