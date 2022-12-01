using FBus.Business.StudentTripManagement.SearchModel;
using FBus.Business.BaseBusiness.CommonModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBus.Business.StudentTripManagement.Interfaces
{
    public interface IStudentTripManagementService
    {
        Task<Response> Create(StudentTripSearchModel model);
        Task<Response> Get(Guid id);
        Task<Response> GetList(Guid? id, DateTime? fromDate, DateTime? toDate, int? status);
        Task<Response> Update(StudentTripUpdateModel model, Guid id);
        Task<Response> CheckIn(string qrCode, Guid studentID);
        Task<Response> FeedBack(FeedBackSearchModel model, Guid id);
        Task<Response> GetCurrent(Guid id);
        Task<Response> Delete(Guid id);
    }
}
