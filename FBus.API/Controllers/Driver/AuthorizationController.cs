using FBus.Business.Authorization.SearchModel;
using FBus.Business.BaseBusiness.Configuration;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using IAuthorizationService = FBus.Business.Authorization.Interfaces.IAuthorizationService;

namespace FBus.API.Controllers.Driver
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
        [Route(ApiVer1Url.Driver.Login)]
        public async Task<IActionResult> Login([FromBody] LoginSearchModel model)
        {
            return SendResponse(await _authorizationService.Login(model, Role.Driver));
        }
    }
}
