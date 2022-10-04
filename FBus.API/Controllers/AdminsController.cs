using FBus.API.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using FBus.Business.Interfaces;

namespace FBus.API.Controllers
{
    [Route("api/" + Version + "/admin/home")]
    [ApiController]
    public class AdminsController : BaseController
    {
        private readonly IAdminService _service;

        public AdminsController(IAdminService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var admins = await _service.GetAdmin();
            return Ok(admins);
        }
    }
}
