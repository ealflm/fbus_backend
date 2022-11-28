using System.Threading.Tasks;
using FBus.Business.DashboardManagement.Interface;
using FBus.Business.DashboardManagement.Models;
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

        [HttpGet]
        [Route(ApiVer1Url.Admin.Dasboard_Booking_Ticket)]
        public async Task<IActionResult> GetNumberOfBookingTickets()
        {
            return SendResponse(await _dashboardService.GetNumberOfBookingTickets());
        }

        [HttpGet]
        [Route(ApiVer1Url.Admin.Dasboard_Complete_Ticket)]
        public async Task<IActionResult> GetNumberOfCompletedTrip()
        {
            return SendResponse(await _dashboardService.GetNumberOfCompletedTrip());
        }

        [HttpGet]
        [Route(ApiVer1Url.Admin.Dasboard_Cancel_Ticket)]
        public async Task<IActionResult> GetNumberOfCancelBookingTickets()
        {
            return SendResponse(await _dashboardService.GetNumberOfCancelBookingTickets());
        }

        [HttpGet]
        [Route(ApiVer1Url.Admin.Dasboard_Ticket_By_Day)]
        public async Task<IActionResult> GetNumberOfTicketsByDay([FromQuery] TicketByDay model)
        {
            return SendResponse(await _dashboardService.GetNumberOfTicketsByDay(model));
        }
    }
}