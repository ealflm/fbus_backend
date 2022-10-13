using System.Threading.Tasks;
using FBus.Business.BaseBusiness.Interfaces;
using Microsoft.Extensions.Configuration;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace FBus.Business.BaseBusiness.Implements
{
    public class SMSService : ISMSService
    {
        protected readonly IConfiguration _configuration;

        public SMSService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendSMSByTwilio(string phone, string password)
        {
            var accountSid = _configuration["Twilio:AccountSID"];
            var authToken = System.Environment.GetEnvironmentVariable("AuthToken") ?? _configuration["Twilio:AuthToken"];
            var phoneFormat = "84" + phone.Substring(1);

            TwilioClient.Init(accountSid, authToken);
            var smsMessage = await MessageResource.CreateAsync(
                body: $"Thông tin đăng nhập: \n Số điện thoại: {phone}\n Mã pin: {password}",
                from: new Twilio.Types.PhoneNumber(_configuration["Twilio:PhoneNumber"]),
                to: new Twilio.Types.PhoneNumber($"+{phoneFormat}")
            );
        }
    }
}