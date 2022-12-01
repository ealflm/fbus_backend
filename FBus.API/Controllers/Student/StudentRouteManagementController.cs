using FBus.Business.StudentTripManagement.Interfaces;
using FBus.Business.StudentTripManagement.SearchModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FBus.API.Controllers.Student
{
    
    
    [ApiController]
    [Authorize]
    [Route(ApiVer1Url.Student.StudentTrip)]
    public class StudentTripManagementController : BaseController
    {

        private readonly IStudentTripManagementService _service;

        public StudentTripManagementController(IStudentTripManagementService service)
        {
            _service = service;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            return SendResponse(await _service.Get(id));
        }

        [HttpGet("current/{id}")]
        public async Task<IActionResult> GetCurrent(Guid id)
        {
            return SendResponse(await _service.GetCurrent(id));
        }


        [HttpGet]
        public async Task<IActionResult> GetList([FromQuery] Guid? id, [FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate, [FromQuery] int? status)
        {
            return SendResponse(await _service.GetList(id, fromDate, toDate, status));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] StudentTripSearchModel model)
        {
            return SendResponse(await _service.Create(model));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            return SendResponse(await _service.Delete(id));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] StudentTripUpdateModel model)
        {
            return SendResponse(await _service.Update(model, id));
        }

        [HttpPatch("feedback/{id}")]
        public async Task<IActionResult> Feedback(Guid id, [FromBody] FeedBackSearchModel model)
        {
            return SendResponse(await _service.FeedBack(model, id));
        }

        [HttpPatch("checkin")]
        public async Task<IActionResult> CheckIn([FromBody] StudentCheckInModel model)
        {
            return SendResponse(await _service.CheckIn(model.QRCode, model.StudentID));
        }
    }
}
