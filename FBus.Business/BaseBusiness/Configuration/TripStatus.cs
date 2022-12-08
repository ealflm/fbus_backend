namespace FBus.Business.BaseBusiness.Configuration
{
    public enum TripStatus
    {
        Disable = 0, // Vô hiệu hóa
        Waiting = 1, // Đang chờ
        NoDriver = 2, // Chưa có tài xế
        Done = 3, // Hoàn thành
        Active = 4, // Đang hoạt động
        NotUsed = 5 // Không sử dụng
    }
}