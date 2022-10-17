using FBus.Business.Authorization.SearchModel;
using FBus.Business.BaseBusiness.CommonModel;
using FBus.Business.BaseBusiness.Configuration;
using System.Threading.Tasks;

namespace FBus.Business.Authorization.Interfaces
{
    public interface IAuthorizationService
    {
        Task<Response> Login(LoginSearchModel model, Role loginType);
        Task<Response> Register(LoginSearchModel model);
        Task<Response> LoginGoogle(string idToken);
        Task<Response> ChangePassword(ModifiedPasswordModel model, Role role);
    }
}
