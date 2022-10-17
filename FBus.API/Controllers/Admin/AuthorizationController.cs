using FBus.Business.Authorization.SearchModel;
using FBus.Business.BaseBusiness.Configuration;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using IAuthorizationService = FBus.Business.Authorization.Interfaces.IAuthorizationService;

namespace FBus.API.Controllers.Admin
{
    [ApiController]
    public class AuthorizationController : BaseController
    {
        private readonly IAuthorizationService _authorizationService;

        public AuthorizationController(IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
        }

        [HttpPost]
        [Route(ApiVer1Url.Admin.Login)]
        public async Task<IActionResult> Login([FromBody] LoginSearchModel model)
        {
            return SendResponse(await _authorizationService.Login(model, Role.Admin));
        }

        [HttpPost]
        [Route(ApiVer1Url.Admin.Register)]
        public async Task<IActionResult> Register([FromForm] LoginSearchModel model)
        {
            return SendResponse(await _authorizationService.Register(model));
        }
        /*
        [HttpGet]
        [Route(ApiVer1Url.Admin.LoginGoogle)]
        public async Task<IActionResult> LoginGoogle([FromQuery] string idToken)
        {
            return SendResponse(await _authorizationService.LoginGoogle(idToken));
        }*/
    }
}
