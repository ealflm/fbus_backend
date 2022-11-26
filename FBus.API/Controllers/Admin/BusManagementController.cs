using System.Threading.Tasks;
using FBus.Business.BusVehicleManagement.Interfaces;
using FBus.Business.BusVehicleManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FBus.API.Controllers.Admin
{
    [ApiController]
    [Authorize]
    [Route(ApiVer1Url.Admin.BusVehicle)]
    public class BusManagementController : BaseController
    {
        private IBusService _busService;

        public BusManagementController(IBusService busService)
        {
            _busService = busService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateBusVehicle(CreateBusModel model)
        {
            return SendResponse(await _busService.Create(model));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBusVehicle(string id, UpdateBusModel model)
        {
            return SendResponse(await _busService.Update(id, model));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDetails(string id)
        {
            return SendResponse(await _busService.GetDetails(id));
        }

        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            return SendResponse(await _busService.GetList());
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            return SendResponse(await _busService.Disable(id));
        }
    }
}