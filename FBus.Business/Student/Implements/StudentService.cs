using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FBus.Business.BaseBusiness.CommonModel;
using FBus.Business.BaseBusiness.Configuration;
using FBus.Business.BaseBusiness.Implements;
using FBus.Business.StudentManagement.Interface;
using FBus.Business.StudentManagement.Models;
using FBus.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FBus.Business.StudentManagement.Implements
{
    public class StudentService : BaseService, IStudentService
    {
        public StudentService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public Task<Response> Disable(string id)
        {
            throw new System.NotImplementedException();
        }

        public async Task<Response> GetDetails(string id)
        {
            var student = await _unitOfWork.StudentRepository.GetById(Guid.Parse(id));
            if (student == null)
            {
                return new()
                {
                    StatusCode = (int)StatusCode.BadRequest,
                    Message = Message.NotFound
                };
            }

            return new()
            {
                StatusCode = (int)StatusCode.Ok,
                Message = Message.GetDetailsSuccess,
                Data = student.AsStudentViewModel()
            };
        }

        public async Task<Response> GetList()
        {
            var list = await _unitOfWork.StudentRepository
                            .Query()
                            .Select(s => s.AsStudentViewModel())
                            .ToListAsync();

            return new()
            {
                StatusCode = (int)StatusCode.Ok,
                Message = Message.GetListSuccess,
                Data = list
            };
        }

        public Task<Response> Update(string id, UpdateStudentModel student)
        {
            throw new System.NotImplementedException();
        }
    }
}