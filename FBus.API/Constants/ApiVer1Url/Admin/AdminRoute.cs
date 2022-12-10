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

        // Notification
        public const string NotificationToken = BaseApiUrl + "/noti-token";
        public const string Notification = BaseApiUrl + "/notification";

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
        public const string AvailableSwappingDriver = BaseApiUrl + "/available-drivers";
        public const string DoSwapDriver = BaseApiUrl + "/swap-driver";

        // Trip
        public const string Trip = BaseApiUrl + "/trip";

        // StudentTrip
        public const string StudentTrip = BaseApiUrl + "/student-trip";

        // Dashboard
        public const string Dashboard = BaseApiUrl + "/dashboard";
        public const string Dasboard_Student = Dashboard + "/students";
        public const string Dasboard_Driver = Dashboard + "/drivers";
        public const string Dasboard_New_Student = Dashboard + "/new-students";
        public const string Dasboard_Bus = Dashboard + "/bus-vehicles";
        public const string Dasboard_Booking_Ticket = Dashboard + "/booking-tickets";
        public const string Dasboard_Complete_Ticket = Dashboard + "/complete-tickets";
        public const string Dasboard_Cancel_Ticket = Dashboard + "/cancel-tickets";
        public const string Dasboard_Ticket_By_Day = Dashboard + "/ticket-by-day";
    }
}