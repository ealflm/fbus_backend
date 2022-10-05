using FBus.Business.StationManagement.Interfaces;
using FBus.Business.StationManagement.SearchModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FBus.API.Controllers.Admin
{
    
    
    [ApiController]
    [Authorize]
    [Route(ApiVer1Url.Admin.Station)]
    public class StationManagementController : BaseController
    {

        private readonly IStationManagementService _service;

        public StationManagementController(IStationManagementService service)
        {
            _service = service;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            return SendResponse(await _service.Get(id));
        }


        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            return SendResponse(await _service.GetList());
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] StationSearchModel model)
        {
            return SendResponse(await _service.Create(model));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            return SendResponse(await _service.Delete(id));
        }
    }
}
