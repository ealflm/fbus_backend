using FBus.API.Constants.ApiVer1Url;

namespace ApiVer1Url
{
    public static class Admin
    {
        // Base url
        public const string Role = "admin";
        public const string BaseApiUrl = BaseRoute.BaseApiUrl + "/" + Role;

        // Authorization
        public const string Login = BaseApiUrl + "/authorization/login";
        public const string Register = BaseApiUrl + "/authorization/register";

        // Station
        public const string Station = BaseApiUrl + "/station";

        // student
        public const string Student = BaseApiUrl + "/student";

        // Student list
        public const string StudentList = Student + "/list";

        // Bus vehicle
        public const string BusVehicle = BaseApiUrl + "/bus";
        
        // Route
        public const string Route = BaseApiUrl + "/route";
        
        // Driver
        public const string Driver = BaseApiUrl + "/driver";

        // Trip
        public const string Trip = BaseApiUrl + "/trip";

    }
}