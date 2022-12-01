using System.Threading.Tasks;
using FBus.Business.BaseBusiness.CommonModel;
using FBus.Business.StudentManagement.Models;

namespace FBus.Business.StudentManagement.Interface
{
    public interface IStudentService
    {
        Task<Response> GetDetails(string id);
        Task<Response> GetByID(string id);
        Task<Response> GetList();
        Task<Response> Update(string id, UpdateStudentModel student);
        Task<Response> Statistics(string id);
        Task<Response> Disable(string id);
        Task<Response> GetDriverLocation(string tripId);
    }
}