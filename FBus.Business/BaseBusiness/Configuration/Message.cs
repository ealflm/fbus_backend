namespace FBus.Business.BaseBusiness.Configuration
{
    public static class Message
    {
        public readonly static string NotFound = "Không tìm thấy nội dung phù hợp với yêu cầu";
        public readonly static string CreatedSuccess = "Tạo mới thành công";
        public readonly static string UpdatedSuccess = "Cập nhật thành công";
        public readonly static string GetDetailsSuccess = "Lấy thông tin chi tiết thành công";
        public readonly static string GetListSuccess = "Lấy danh sách thành công";
        public readonly static string AlreadyExist = "Đối tượng đã tồn tại";
        public readonly static string WrongUsername = "Tài khoản không chính xác";
        public readonly static string WrongPassword = "Mật khẩu không chính xác";
        public readonly static string ChangePasswordSuccess = "Đổi mật khẩu thành công";
        public readonly static string ErrorExecution = "Thao tác thất bại";

        public static string CustomContent(string content)
        {
            return content;
        }
    }
}