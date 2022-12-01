using FBus.Business.TripManagement.Interfaces;
using FBus.Business.TripManagement.SearchModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FBus.API.Controllers.Admin
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
        [Route(ApiVer1Url.Admin.Trip + "/{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            return SendResponse(await _service.Get(id));
        }

        [HttpGet]
        [Route(ApiVer1Url.Admin.Trip)]
        public async Task<IActionResult> GetList()
        {
            return SendResponse(await _service.GetList(null, null));
        }

        [HttpPost]
        [Route(ApiVer1Url.Admin.Trip)]
        public async Task<IActionResult> Create([FromBody] TripSearchModel model)
        {
            return SendResponse(await _service.Create(model));
        }

        [HttpDelete]
        [Route(ApiVer1Url.Admin.Trip + "/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            return SendResponse(await _service.Delete(id));
        }

        [HttpPut]
        [Route(ApiVer1Url.Admin.Trip + "/{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] TripUpdateModel model)
        {
            return SendResponse(await _service.Update(model, id));
        }

        [HttpGet]
        [Route(ApiVer1Url.Admin.AvailableSwappingDriver)]
        public async Task<IActionResult> GetAvailableDriver([FromQuery] AvailableSwappingDriverModel model)
        {
            return SendResponse(await _service.GetAvailabelSwappingDriverList(model));
        }

        [HttpPost]
        [Route(ApiVer1Url.Admin.DoSwapDriver)]
        public async Task<IActionResult> DoSwapDriver([FromBody] SwapDriverModel model)
        {
            return SendResponse(await _service.DoSwapDriver(model));
        }
    }
}
