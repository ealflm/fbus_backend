using FBus.Business.StudentTripManagement.Interfaces;
using FBus.Business.StudentTripManagement.SearchModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FBus.API.Controllers.Admin
{
    
    
    [ApiController]
    [Authorize]
    [Route(ApiVer1Url.Admin.StudentTrip)]
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


        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            return SendResponse(await _service.GetList(null, null, null,null));
        }

       /* [HttpPost]
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
        }*/
    }
}
