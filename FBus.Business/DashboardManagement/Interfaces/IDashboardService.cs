using System.Threading.Tasks;
using FBus.Business.BaseBusiness.CommonModel;
using FBus.Business.DashboardManagement.Models;

namespace FBus.Business.DashboardManagement.Interface
{
    public interface IDashboardService
    {
        Task<Response> GetNumberOfStudentAccounts(); // Tổng số lượng tài khoản học sinh trong hệ thống
        Task<Response> GetNumberOfDriverAccounts(); // Tổng số lượng tài khoản tài xế trong hệ thống
        Task<Response> GetNumberOfBusVehicles(); // Tổng số lượng xe trong hệ thống
        Task<Response> GetNumberOfNewUsers(); // Tổng số lượng người dùng mới (học sinh) trong ngày
        Task<Response> GetNumberOfCompletedTrip(); // Tổng số lượng vé đã hoàn thành
        Task<Response> GetNumberOfBookingTickets(); // Tổng số lượng vé đã đặt
        Task<Response> GetNumberOfCancelBookingTickets(); // Tổng số lượng vé đã bị hủy
        Task<Response> GetNumberOfTicketsByDay(TicketByDay model);
    }
}