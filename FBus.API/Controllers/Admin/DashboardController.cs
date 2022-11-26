using System.Threading.Tasks;
using FBus.Business.DashboardManagement.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FBus.API.Controllers.Admin
{
    [ApiController]
    [Authorize]
    public class DashboardManagementController : BaseController
    {
        private readonly IDashboardService _dashboardService;

        public DashboardManagementController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet]
        [Route(ApiVer1Url.Admin.Dasboard_Student)]
        public async Task<IActionResult> GetNumberOfStudentAccounts()
        {
            return SendResponse(await _dashboardService.GetNumberOfStudentAccounts());
        }

        [HttpGet]
        [Route(ApiVer1Url.Admin.Dasboard_Driver)]
        public async Task<IActionResult> GetNumberOfDriverAccounts()
        {
            return SendResponse(await _dashboardService.GetNumberOfDriverAccounts());
        }

        [HttpGet]
        [Route(ApiVer1Url.Admin.Dasboard_Bus)]
        public async Task<IActionResult> GetNumberOfBusVehicles()
        {
            return SendResponse(await _dashboardService.GetNumberOfBusVehicles());
        }

        [HttpGet]
        [Route(ApiVer1Url.Admin.Dasboard_New_Student)]
        public async Task<IActionResult> GetNumberOfNewUsers()
        {
            return SendResponse(await _dashboardService.GetNumberOfNewUsers());
        }
    }
}