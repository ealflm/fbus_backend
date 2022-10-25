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
            return SendResponse(await _service.GetList(null));
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
    }
}
