using System.Threading.Tasks;
using FBus.Business.BaseBusiness.Configuration;
using FBus.Business.BaseBusiness.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FBus.API.Controllers.Admin
{
    [ApiController]
    [Authorize]
    public class NotificationController : BaseController
    {
        private readonly INotificationService _notiService;

        public NotificationController(INotificationService notiService)
        {
            _notiService = notiService;
        }

        [HttpPost]
        [Route(ApiVer1Url.Admin.NotificationToken)]
        public async Task<IActionResult> SaveNotiToken([FromBody] NotiTokenModel model)
        {
            return SendResponse(await _notiService.SaveNotiToken(model, Role.Admin));
        }
    }
}