using System.Threading.Tasks;
using FBus.Business.DriverManagement.Interfaces;
using FBus.Business.DriverManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FBus.API.Controllers.Admin
{
    [ApiController]
    [Authorize]
    public class DriverManagementController : BaseController
    {
        private readonly IDriverService _driverService;
        public DriverManagementController(IDriverService driverService)
        {
            _driverService = driverService;
        }

        [HttpGet]
        [Route(ApiVer1Url.Admin.Driver + "/{id}")]
        public async Task<IActionResult> GetDetails(string id)
        {
            return SendResponse(await _driverService.GetDetails(id));
        }

        [HttpGet]
        [Route(ApiVer1Url.Admin.Driver)]
        public async Task<IActionResult> GetList()
        {
            return SendResponse(await _driverService.GetList());
        }

        [HttpPost]
        [Route(ApiVer1Url.Admin.Driver)]
        public async Task<IActionResult> CreateDriver([FromForm] CreateDriverModel model)
        {
            return SendResponse(await _driverService.Create(model));
        }
    }
}