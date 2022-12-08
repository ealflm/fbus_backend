namespace FBus.Business.BaseBusiness.Configuration
{
    public enum DriverStatus
    {
        Disable = 0, // Vô hiệu hóa
        Active = 1, // Chưa có tuyến
        Assigned = 2, // Đang hoạt động
        Running = 3, // Đang chạy
    }
}