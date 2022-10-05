namespace FBus.Business.BaseBusiness.Configuration
{
    public enum StatusCode
    {
        Ok = 200, // Mã code dành cho các phương thức get
        Success = 201, // Mã code dành cho các phương thức post, put, patch, delete
        NoContent = 204, // Không cần nội dung của response từ server, chỉ cần biết mã code khi thực hiện thành công request
        BadRequest = 400, // yêu cầu không hợp lệ từ phía client
        NotFound = 404, // không tìm thấy nội dung phù hợp với yêu cầu
        UnAuthorized = 401, // Chưa xác minh đăng nhập
        Forbbiden = 403, // đã xác minh đăng nhập và không có quyền truy cập nguồn tài nguyên
        ServerError = 500, // lỗi từ hệ thống
    }
}