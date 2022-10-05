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

        // Student detail
        public const string StudentList = BaseApiUrl + "/list";
    }
}
