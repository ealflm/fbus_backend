using FBus.Business.TripManagement.Interfaces;
using FBus.Business.TripManagement.SearchModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FBus.API.Controllers.Student
{
    
    
    [ApiController]
    [Authorize]
    [Route(ApiVer1Url.Student.Trip)]
    public class TripManagementController : BaseController
    {

        private readonly ITripManagementService _service;

        public TripManagementController(ITripManagementService service)
        {
            _service = service;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            return SendResponse(await _service.Get(id));
        }

        [HttpGet]
        public async Task<IActionResult> GetList([FromQuery] DateTime? date, [FromQuery] Guid id)
        {
            return SendResponse(await _service.GetListByRoute(id,date));
        }

       /* [HttpPost]
        public async Task<IActionResult> Create([FromBody] TripSearchModel model)
        {
            return SendResponse(await _service.Create(model));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            return SendResponse(await _service.Delete(id));
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id,[FromBody] TripUpdateModel model)
        {
            return SendResponse(await _service.Update(model, id));
        }*/
    }
}
