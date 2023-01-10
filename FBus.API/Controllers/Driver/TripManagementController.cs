using FBus.Business.BaseBusiness.Configuration;
using FBus.Business.TripManagement.Interfaces;
using FBus.Business.TripManagement.SearchModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FBus.API.Controllers.Driver
{


    [ApiController]
    [Authorize]
    public class TripManagementController : BaseController
    {

        private readonly ITripManagementService _service;

        public TripManagementController(ITripManagementService service)
        {
            _service = service;
        }

        [HttpGet]
        [Route(ApiVer1Url.Driver.Trip + "/{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            return SendResponse(await _service.Get(id));
        }

        [HttpGet]
        [Route(ApiVer1Url.Driver.Trip)]
        public async Task<IActionResult> GetList()
        {
            return SendResponse(await _service.GetList(null, null));
        }

        [HttpGet]
        [Route(ApiVer1Url.Driver.Trip +"/feedback/{id}")]
        public async Task<IActionResult> GetFeedback(Guid id)
        {
            return SendResponse(await _service.GetFeedback(id));
        }

        [HttpGet]
        [Route(ApiVer1Url.Driver.Trip + "/current/{id}")]
        public async Task<IActionResult> GetCurrent(Guid id)
        {
            return SendResponse(await _service.GetCurrent(id));
        }

        [HttpGet]
        [Route(ApiVer1Url.Driver.Trip + "/studentTrip/{id}")]
        public async Task<IActionResult> GetStudentTrip(Guid id)
        {
            return SendResponse(await _service.GetStudentTrips(id));
        }
        /* [HttpPost]
         public async Task<IActionResult> Create([FromBody] TripSearchModel model)
         {
             return SendResponse(await _service.Create(model));
         }

         [HttpDelete("{id}")]
         public async Task<IActionResult> Delete(Guid id)
         {
             return SendResponse(await _service.Delete(id));
         }
         [HttpPut("{id}")]
         public async Task<IActionResult> Update(Guid id,[FromBody] TripUpdateModel model)
         {
             return SendResponse(await _service.Update(model, id));
         }*/

        [HttpGet]
        [Route(ApiVer1Url.Driver.HistoricalTrip + "/{id}")]
        public async Task<IActionResult> GetHistoricalTrip(string id)
        {
            return SendResponse(await _service.GetHistoricalTrip(id, Role.Driver));
        }

        [HttpGet]
        [Route(ApiVer1Url.Driver.TripSchedules + "/{id}")]
        public async Task<IActionResult> GetTripSchedules(string id)
        {
            return SendResponse(await _service.GetDriverTripSchedules(id));
        }

        [HttpGet]
        [Route(ApiVer1Url.Driver.CheckAvailableRequestTime)]
        public async Task<IActionResult> CheckAvailableRequestTime(RequestTimeModel model)
        {
            return SendResponse(await _service.CheckAvailableRequestTime(model));
        }

        [HttpPost]
        [Route(ApiVer1Url.Driver.SendRequestToSwapDriver)]
        public async Task<IActionResult> SendRequestToSwapDriver([FromBody] RequestSwapDriverModel model)
        {
            return SendResponse(await _service.SendRequestToSwapDriver(model));
        }

        [HttpPatch]
        [Route(ApiVer1Url.Driver.CheckIn)]
        public async Task<IActionResult> CheckInTripForDriver([FromBody] DriverCheckinModel model)
        {
            return SendResponse(await _service.CheckInTripForDriver(model.QRCode, model.DriverId));
        }
    }
}
