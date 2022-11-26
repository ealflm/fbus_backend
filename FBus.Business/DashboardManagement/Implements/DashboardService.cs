using System;
using System.Linq;
using System.Threading.Tasks;
using FBus.Business.BaseBusiness.CommonModel;
using FBus.Business.BaseBusiness.Configuration;
using FBus.Business.BaseBusiness.Implements;
using FBus.Business.DashboardManagement.Interface;
using FBus.Business.DashboardManagement.Models;
using FBus.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FBus.Business.DashboardManagement.Implements
{
    public class DashboardService : BaseService, IDashboardService
    {
        public DashboardService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<Response> GetNumberOfStudentAccounts()
        {
            var students = await _unitOfWork.StudentRepository
                                .Query()
                                .ToListAsync();

            NormalStatus data = new NormalStatus();

            foreach (var item in students)
            {
                if (item.Status == (int)StudentStatus.Active)
                {
                    data.Actived += 1;
                    continue;
                }

                data.Disabled += 1;
            }

            data.All = data.Actived + data.Disabled;

            return new()
            {
                StatusCode = (int)StatusCode.Ok,
                Message = Message.CustomContent("Thành công"),
                Data = data
            };

        }

        public Task<Response> GetNumberOfBookingTickets()
        {
            throw new System.NotImplementedException();
        }

        public async Task<Response> GetNumberOfBusVehicles()
        {
            var buses = await _unitOfWork.BusRepository
                                .Query()
                                .ToListAsync();

            NormalStatus data = new NormalStatus();

            foreach (var item in buses)
            {
                if (item.Status == (int)BusStatus.Active)
                {
                    data.Actived += 1;
                    continue;
                }

                data.Disabled += 1;
            }

            data.All = data.Actived + data.Disabled;

            return new()
            {
                StatusCode = (int)StatusCode.Ok,
                Message = Message.CustomContent("Thành công"),
                Data = data
            };
        }

        public Task<Response> GetNumberOfCancelBookingTickets()
        {
            throw new System.NotImplementedException();
        }

        public Task<Response> GetNumberOfCompletedTrip()
        {
            throw new System.NotImplementedException();
        }

        public async Task<Response> GetNumberOfNewUsers()
        {
            var newStudents = await _unitOfWork.StudentRepository
                                .Query()
                                .Where(x =>
                                        x.CreatedDate.Value.Date.CompareTo(DateTime.UtcNow.Date) == 0 &&
                                        x.CreatedDate.Value.Month.CompareTo(DateTime.UtcNow.Month) == 0 &&
                                        x.CreatedDate.Value.Year.CompareTo(DateTime.UtcNow.Year) == 0
                                    )
                                .ToListAsync();

            NormalStatus data = new NormalStatus();

            foreach (var item in newStudents)
            {
                if (item.Status == (int)DriverStatus.Active)
                {
                    data.Actived += 1;
                    continue;
                }

                data.Disabled += 1;
            }

            data.All = data.Actived + data.Disabled;

            return new()
            {
                StatusCode = (int)StatusCode.Ok,
                Message = Message.CustomContent("Thành công"),
                Data = data
            };
        }

        public async Task<Response> GetNumberOfDriverAccounts()
        {
            var drivers = await _unitOfWork.DriverRepository
                                .Query()
                                .ToListAsync();

            NormalStatus data = new NormalStatus();

            foreach (var item in drivers)
            {
                if (item.Status == (int)DriverStatus.Active)
                {
                    data.Actived += 1;
                    continue;
                }

                data.Disabled += 1;
            }

            data.All = data.Actived + data.Disabled;

            return new()
            {
                StatusCode = (int)StatusCode.Ok,
                Message = Message.CustomContent("Thành công"),
                Data = data
            };
        }
    }
}