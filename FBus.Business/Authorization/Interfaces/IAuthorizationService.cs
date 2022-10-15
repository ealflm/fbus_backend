using FBus.Business.Authorization.SearchModel;
using FBus.Business.BaseBusiness.CommonModel;
using System.Threading.Tasks;

namespace FBus.Business.Authorization.Interfaces
{
    public interface IAuthorizationService
    {
        Task<Response> Login(LoginSearchModel model, Login loginType);
        Task<Response> Register(LoginSearchModel model);
        Task<Response> LoginGoogle(string idToken);
        Task<Response> ChangePassword(ModifiedPasswordModel model, Login role);
    }
}
