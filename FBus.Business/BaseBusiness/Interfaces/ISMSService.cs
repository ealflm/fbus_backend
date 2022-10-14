using System.Threading.Tasks;

namespace FBus.Business.BaseBusiness.Interfaces
{
    public interface ISMSService
    {
        Task SendSMSByTwilio(string phone, string password);
    }
}