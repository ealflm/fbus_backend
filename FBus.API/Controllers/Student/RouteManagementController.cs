using FBus.Business.RouteManagement.Interfaces;
using FBus.Business.RouteManagement.SearchModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FBus.API.Controllers.Student
{
    
    
    [ApiController]
    [Authorize]
    [Route(ApiVer1Url.Student.Route)]
    public class RouteManagementController : BaseController
    {

        private readonly IRouteManagementService _service;

        public RouteManagementController(IRouteManagementService service)
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

       /* [HttpPost]
        public async Task<IActionResult> Create([FromBody] RouteSearchModel model)
        {
            return SendResponse(await _service.Create(model));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            return SendResponse(await _service.Delete(id));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] StationUpdateModel model)
        {
            return SendResponse(await _service.Update(model, id));
        }*/
    }
}
