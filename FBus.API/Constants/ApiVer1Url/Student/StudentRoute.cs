using FBus.API.Constants.ApiVer1Url;

namespace ApiVer1Url
{
    public static class Student
    {
        // Base url
        public const string Role = "student";
        public const string BaseApiUrl = BaseRoute.BaseApiUrl + "/" + Role;

        // Authorization
        public const string Login = BaseApiUrl + "/authorization/login";

        // Station
        public const string Station = BaseApiUrl + "/station";

        // Route
        public const string Route = BaseApiUrl + "/route";
        // Trip
        public const string Trip = BaseApiUrl + "/trip";

        // StudentTrip
        public const string StudentTrip = BaseApiUrl + "/student-trip";
    }
}
