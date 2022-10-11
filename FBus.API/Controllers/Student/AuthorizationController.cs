using FBus.Business.Authorization.Interfaces;
using FBus.Business.Authorization.SearchModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IAuthorizationService = FBus.Business.Authorization.Interfaces.IAuthorizationService;

namespace FBus.API.Controllers.Student
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
    [Route(ApiVer1Url.Student.Login)]
    public async Task<IActionResult> LoginGoogle([FromQuery] string idToken)
    {
      return SendResponse(await _authorizationService.LoginGoogle(idToken));
    }
  }
}
