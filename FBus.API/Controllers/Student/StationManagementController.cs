using FBus.Business.StationManagement.Interfaces;
using FBus.Business.StationManagement.SearchModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FBus.API.Controllers.Student
{
    
    
    [ApiController]
    [Authorize]
    [Route(ApiVer1Url.Student.Station)]
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
    }
}
