using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using FBus.Business.BaseBusiness.CommonModel;
using FBus.Business.BaseBusiness.Configuration;
using FBus.Business.BaseBusiness.Implements;
using FBus.Business.BaseBusiness.Interfaces;
using FBus.Business.StudentManagement.Interface;
using FBus.Business.StudentManagement.Models;
using FBus.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FBus.Business.StudentManagement.Implements
{
    public class StudentService : BaseService, IStudentService
    {
        private IAzureBlobService _azureBlobService;

        public StudentService(IUnitOfWork unitOfWork, IAzureBlobService azureBlobService) : base(unitOfWork)
        {
            _azureBlobService = azureBlobService;
        }

        public async Task<Response> Disable(string id)
        {
            var student = await _unitOfWork.StudentRepository.GetById(Guid.Parse(id));
            if (student == null)
            {
                return new()
                {
                    StatusCode = (int)StatusCode.NotFound,
                    Message = Message.NotFound
                };
            }

            student.Status = (int)StudentStatus.Disable;
            _unitOfWork.StudentRepository.Update(student);
            await _unitOfWork.SaveChangesAsync();

            return new()
            {
                StatusCode = (int)StatusCode.Success,
                Message = Message.UpdatedSuccess,
            };
        }

        public async Task<Response> GetDetails(string id)
        {
            var student = await _unitOfWork.StudentRepository.GetById(Guid.Parse(id));
            if (student == null)
            {
                return new()
                {
                    StatusCode = (int)StatusCode.NotFound,
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

        public async Task<Response> Update(string id, UpdateStudentModel student)
        {
            var stud = await _unitOfWork.StudentRepository.GetById(Guid.Parse(id));
            if (stud == null)
            {
                return new()
                {
                    StatusCode = (int)StatusCode.NotFound,
                    Message = Message.NotFound
                };
            }

            stud.FullName = UpdateTypeOfNullAbleObject<string>(stud.FullName, student.FullName);
            stud.Phone = UpdateTypeOfNullAbleObject<string>(stud.Phone, student.Phone);
            stud.Address = UpdateTypeOfNullAbleObject<string>(stud.Address, student.Address);
            stud.PhotoUrl = await _azureBlobService.UploadFile(student.UploadFile, AzureBlobContainer.Student);
            stud.NotifyToken = UpdateTypeOfNullAbleObject<string>(stud.NotifyToken, student.NotifyToken);
            stud.AutomaticScheduling = UpdateTypeOfNotNullAbleObject<bool>(stud.AutomaticScheduling, student.AutomaticScheduling);
            stud.Status = UpdateTypeOfNotNullAbleObject<int>(stud.Status, student.Status);

            _unitOfWork.StudentRepository.Update(stud);
            await _unitOfWork.SaveChangesAsync();

            return new()
            {
                StatusCode = (int)StatusCode.Success,
                Message = Message.UpdatedSuccess,
            };
        }
    }
}