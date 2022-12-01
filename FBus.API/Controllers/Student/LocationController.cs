using System.Threading.Tasks;
using FBus.Business.DriverManagement.Interfaces;
using FBus.Business.DriverManagement.Models;
using FBus.Business.StudentManagement.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FBus.API.Controllers.Student
{
    [ApiController]
    [Authorize]
    public class LocationController : BaseController
    {
        private readonly IStudentService _studentService;

        public LocationController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        [HttpGet]
        [Route(ApiVer1Url.Student.Location)]
        public async Task<IActionResult> TrackingLocation([FromQuery] string tripId)
        {
            return SendResponse(await _studentService.GetDriverLocation(tripId));
        }
    }
}