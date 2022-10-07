using FBus.API.Constants.ApiVer1Url;

namespace ApiVer1Url
{
    public static class Driver
    {
        // Base url
        public const string Role = "driver";
        public const string BaseApiUrl = BaseRoute.BaseApiUrl + "/" + Role;

        // Authorization
        public const string Login = BaseApiUrl + "/authorization/login";

        // Station
        public const string Station = BaseApiUrl + "/station";
    }
}
