using FBus.Business.TripManagement.SearchModel;
using FBus.Business.BaseBusiness.CommonModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FBus.Business.BaseBusiness.Configuration;
using FBus.Data.Interfaces;

namespace FBus.Business.TripManagement.Interfaces
{
    public interface ITripManagementService
    {
        public IUnitOfWork UnitOfWork { get; }
        Task<Response> Create(TripSearchModel model);
        Task<Response> Get(Guid id);
        Task<Response> GetListByRoute(Guid? id, DateTime? date);
        Task<Response> GetList(Guid? busId, Guid? driverId);
        Task<Response> Update(TripUpdateModel model, Guid id);
        Task<Response> Delete(Guid id);
        Task<Response> GetHistoricalTrip(string id, Role role);
        Task<Response> GetDriverTripSchedules(string id);
        Task<Response> SendRequestToSwapDriver(RequestSwapDriverModel model);
        Task<Response> DoSwapDriver(SwapDriverModel model);
        Task<Response> CheckAvailableRequestTime(RequestTimeModel model);
        Task<Response> GetAvailabelSwappingDriverList(AvailableSwappingDriverModel model);
        Task<Response> CheckInTripForDriver(string qrCode, string driverId);
        Task<Response> MakeReadRequest(string id); 
        Task<Response> GetFeedback(Guid id);
        Task<Response> GetCurrent(Guid id);

        Task<Response> GetStudentTrips(Guid id);
    }
}
