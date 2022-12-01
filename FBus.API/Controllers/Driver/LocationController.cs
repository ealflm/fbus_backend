using System.Threading.Tasks;
using FBus.Business.DriverManagement.Interfaces;
using FBus.Business.DriverManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FBus.API.Controllers.Driver
{
    [ApiController]
    [Authorize]
    public class LocationController : BaseController
    {
        private readonly IDriverService _driverService;

        public LocationController(IDriverService driverService)
        {
            _driverService = driverService;
        }

        [HttpPost]
        [Route(ApiVer1Url.Driver.TrackingLocation)]
        public async Task<IActionResult> TrackingLocation([FromBody] TrackingLocationModel model)
        {
            return SendResponse(await _driverService.TrackingLocation(model));
        }
    }
}