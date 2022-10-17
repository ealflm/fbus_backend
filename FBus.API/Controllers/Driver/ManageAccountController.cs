using System.Threading.Tasks;
using FBus.Business.Authorization.SearchModel;
using FBus.Business.BaseBusiness.Configuration;
using FBus.Business.DriverManagement.Interfaces;
using FBus.Business.DriverManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FBus.API.Controllers.Driver
{
    [ApiController]
    [Authorize]
    public class ManageAccountController : BaseController
    {
        private IDriverService _driverService;
        private FBus.Business.Authorization.Interfaces.IAuthorizationService _authorizationService;
        public ManageAccountController(IDriverService driverService, FBus.Business.Authorization.Interfaces.IAuthorizationService authorizationService)
        {
            _driverService = driverService;
            _authorizationService = authorizationService;
        }

        [HttpPut]
        [Route(ApiVer1Url.Driver.Profile + "/{id}")]
        public async Task<IActionResult> UpdateProfile(string id, [FromForm] UpdateDriverModel model)
        {
            return SendResponse(await _driverService.Update(id, model));
        }

        [HttpPut]
        [Route(ApiVer1Url.Driver.ChangePassword)]
        public async Task<IActionResult> ChangePassword(ModifiedPasswordModel model)
        {
            return SendResponse(await _authorizationService.ChangePassword(model, Role.Driver));
        }
    }
}