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

        // Update Profile
        public const string Profile = BaseApiUrl + "/profile";

        // Change password
        public const string ChangePassword = BaseApiUrl + "/change-password";

        // Route
        public const string Route = BaseApiUrl + "/route";

        // Trip
        public const string Trip = BaseApiUrl + "/trip";

        // Historical trip
        public const string HistoricalTrip = Trip + "/history";

        // Trip Schedules
        public const string TripSchedules = Trip + "/schedule";

        // StudentTrip
        public const string StudentTrip = BaseApiUrl + "/student-trip";
    }
}
