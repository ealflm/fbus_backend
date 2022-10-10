using System.Threading.Tasks;
using FBus.Business.BaseBusiness.CommonModel;
using FBus.Business.BusVehicle.Models;

namespace FBus.Business.BusVehicle.Interfaces
{
    public interface IBusService
    {
        Task<Response> GetDetails(string id);
        Task<Response> GetList();
        Task<Response> Create(CreateBusModel model);
        Task<Response> Update(string id, UpdateBusModel model);
        Task<Response> Disable(string id);
    }
}