using System.Threading.Tasks;
using FBus.Business.BaseBusiness.CommonModel;
using FBus.Business.DriverManagement.Models;

namespace FBus.Business.DriverManagement.Interfaces
{
    public interface IDriverService
    {
        Task<Response> Create(CreateDriverModel model);
        Task<Response> Update(string id, UpdateDriverModel model, bool isAdminRole = false);
        Task<Response> Disable(string id);
        Task<Response> GetDetails(string id);
        Task<Response> GetList();
        Task<Response> TrackingLocation(TrackingLocationModel model);
        Task<Response> Statistics(string id);
        Response GenCode(string plainText);
    }
}