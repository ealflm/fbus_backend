using System;
using System.Collections;
using System.Collections.Generic;
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
    public class NormalizeList
    {
        public decimal Key { get; set; }
        public List<int> Value { get; set; }
        public List<PriceObject> Price { get; set; }
    }

    public class PriceObject
    {
        public decimal Month { get; set; }
        public decimal Price { get; set; }
    }

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

        public async Task<Response> GetNumberOfBookingTickets()
        {
            var tickets = await _unitOfWork.StudentTripRepository
                .Query()
                .Where(x => x.Status == (int)StudentTripStatus.Active)
                .Select(x => new
                {
                    StudentTripId = x.StudentTripId,
                    CreatedDate = x.CreateDate
                })
                .ToListAsync();

            var normalizeList = tickets.GroupBy(
                x => x.CreatedDate.Year,
                x => x.CreatedDate,
                (key, value) => new NormalizeList()
                {
                    Key = key,
                    Value = value.Select(x => x.Month).ToList()
                }
            );

            Hashtable hashtable = new Hashtable();
            Dictionary<decimal, object> dictionary = new Dictionary<decimal, object>();
            DoProcess(dictionary, hashtable, normalizeList);

            return new()
            {
                StatusCode = (int)StatusCode.Ok,
                Message = Message.CustomContent("Thành công"),
                Data = dictionary
            };
        }

        public async Task<Response> GetNumberOfCompletedTrip()
        {
            var tickets = await _unitOfWork.StudentTripRepository
                .Query()
                .Where(x => x.Status == (int)StudentTripStatus.Passed)
                .Select(x => new
                {
                    StudentTripId = x.StudentTripId,
                    CreatedDate = x.CreateDate
                })
                .ToListAsync();

            var normalizeList = tickets.GroupBy(
                x => x.CreatedDate.Year,
                x => x.CreatedDate,
                (key, value) => new NormalizeList()
                {
                    Key = key,
                    Value = value.Select(x => x.Month).ToList()
                }
            );

            Hashtable hashtable = new Hashtable();
            Dictionary<decimal, object> dictionary = new Dictionary<decimal, object>();
            DoProcess(dictionary, hashtable, normalizeList);

            return new()
            {
                StatusCode = (int)StatusCode.Ok,
                Message = Message.CustomContent("Thành công"),
                Data = dictionary
            };
        }

        public async Task<Response> GetNumberOfCancelBookingTickets()
        {
            var tickets = await _unitOfWork.StudentTripRepository
                .Query()
                .Where(x => x.Status == (int)StudentTripStatus.Canceled)
                .Select(x => new
                {
                    StudentTripId = x.StudentTripId,
                    CreatedDate = x.CreateDate
                })
                .ToListAsync();

            var normalizeList = tickets.GroupBy(
                x => x.CreatedDate.Year,
                x => x.CreatedDate,
                (key, value) => new NormalizeList()
                {
                    Key = key,
                    Value = value.Select(x => x.Month).ToList()
                }
            );

            Hashtable hashtable = new Hashtable();
            Dictionary<decimal, object> dictionary = new Dictionary<decimal, object>();
            DoProcess(dictionary, hashtable, normalizeList);

            return new()
            {
                StatusCode = (int)StatusCode.Ok,
                Message = Message.CustomContent("Thành công"),
                Data = dictionary
            };
        }

        public async Task<Response> GetNumberOfTicketsByDay(TicketByDay model)
        {
            var year = model.Year ?? DateTime.UtcNow.Year;
            var month = model.Month ?? DateTime.UtcNow.Month;
            var countDays = DateTime.DaysInMonth(year, month);

            Hashtable result = new Hashtable();
            result.Add("booking", "");
            result.Add("completed", "");
            result.Add("canceled", "");

            var studentTripBooking = await _unitOfWork.StudentTripRepository
                                .Query()
                                .Where(x => x.CreateDate.Month == month && x.CreateDate.Year == year && x.Status == (int)StudentTripStatus.Active)
                                .ToListAsync();

            var studentTripComplete = await _unitOfWork.StudentTripRepository
                                .Query()
                                .Where(x => x.ModifyDate.Month == month && x.ModifyDate.Year == year && x.Status == (int)StudentTripStatus.Passed)
                                .ToListAsync();

            var studentTripCanceled = await _unitOfWork.StudentTripRepository
                                .Query()
                                .Where(x => x.ModifyDate.Month == month && x.ModifyDate.Year == year && x.Status == (int)StudentTripStatus.Canceled)
                                .ToListAsync();

            Hashtable booking = new Hashtable();
            Hashtable completed = new Hashtable();
            Hashtable canceled = new Hashtable();

            for (int i = 1; i <= countDays; i++)
            {
                // Tính số vé đã đặt trong ngày
                booking.Add(i, 0);
                foreach (var item in studentTripBooking)
                {
                    if (item.CreateDate.Day == i)
                    {
                        booking[i] = int.Parse(booking[i].ToString()) + 1;
                    }
                }

                // Tính số vé đã hoàn thành trong ngày
                completed.Add(i, 0);
                foreach (var item in studentTripComplete)
                {
                    if (item.ModifyDate.Day == i)
                    {
                        completed[i] = int.Parse(completed[i].ToString()) + 1;
                    }
                }

                // Tính số vé đã hủy trong ngày
                canceled.Add(i, 0);
                foreach (var item in studentTripCanceled)
                {
                    if (item.ModifyDate.Day == i)
                    {
                        canceled[i] = int.Parse(canceled[i].ToString()) + 1;
                    }
                }
            }

            result["booking"] = booking;
            result["completed"] = completed;
            result["canceled"] = canceled;

            return new()
            {
                StatusCode = (int)StatusCode.Ok,
                Message = Message.CustomContent("Thành công"),
                Data = result
            };
        }

        private void DoProcess(Dictionary<decimal, object> dictionary, Hashtable hashtable, IEnumerable<NormalizeList> normalizeList,
                                bool isCalculateRevenue = false, decimal value = 1)
        {
            foreach (var item in normalizeList)
            {
                // clear oldest data
                hashtable.Clear();

                // Create hashtable list with "MONTH" is "key" and default "value" is "0"
                for (int i = 0; i < 12; i++)
                {
                    hashtable.Add(i + 1, 0);
                }

                // Check existed key, If not, added to dictionary
                if (!dictionary.ContainsKey(item.Key))
                {
                    dictionary.Add(item.Key, hashtable);
                }

                dynamic list = isCalculateRevenue ? item.Price : item.Value;

                // Count
                foreach (var month in list)
                {
                    var choice = isCalculateRevenue ? (int)month.Month : month;
                    switch (choice)
                    {
                        case (int)Months.Jan:
                            {
                                var temp = isCalculateRevenue ? (decimal)month.Price : value;
                                hashtable[(int)Months.Jan] = Int32.Parse(hashtable[(int)Months.Jan].ToString()) + temp;
                                break;
                            }
                        case (int)Months.Feb:
                            {
                                var temp = isCalculateRevenue ? (decimal)month.Price : value;
                                hashtable[(int)Months.Feb] = Int32.Parse(hashtable[(int)Months.Feb].ToString()) + temp;
                                break;
                            };
                        case (int)Months.Mar:
                            {
                                var temp = isCalculateRevenue ? (decimal)month.Price : value;
                                hashtable[(int)Months.Mar] = Int32.Parse(hashtable[(int)Months.Mar].ToString()) + temp;
                                break;
                            };
                        case (int)Months.Apr:
                            {
                                var temp = isCalculateRevenue ? (decimal)month.Price : value;
                                hashtable[(int)Months.Apr] = Int32.Parse(hashtable[(int)Months.Apr].ToString()) + temp;
                                break;
                            };
                        case (int)Months.May:
                            {
                                var temp = isCalculateRevenue ? (decimal)month.Price : value;
                                hashtable[(int)Months.May] = Int32.Parse(hashtable[(int)Months.May].ToString()) + temp;
                                break;
                            };
                        case (int)Months.Jun:
                            {
                                var temp = isCalculateRevenue ? (decimal)month.Price : value;
                                hashtable[(int)Months.Jun] = Int32.Parse(hashtable[(int)Months.Jun].ToString()) + temp;
                                break;
                            };
                        case (int)Months.Jul:
                            {
                                var temp = isCalculateRevenue ? (decimal)month.Price : value;
                                hashtable[(int)Months.Jul] = Int32.Parse(hashtable[(int)Months.Jul].ToString()) + temp;
                                break;
                            };
                        case (int)Months.Aug:
                            {
                                var temp = isCalculateRevenue ? (decimal)month.Price : value;
                                hashtable[(int)Months.Aug] = Int32.Parse(hashtable[(int)Months.Aug].ToString()) + temp;
                                break;
                            };
                        case (int)Months.Sep:
                            {
                                var temp = isCalculateRevenue ? (decimal)month.Price : value;
                                hashtable[(int)Months.Sep] = Int32.Parse(hashtable[(int)Months.Sep].ToString()) + temp;
                                break;
                            };
                        case (int)Months.Oct:
                            {
                                var temp = isCalculateRevenue ? (decimal)month.Price : value;
                                hashtable[(int)Months.Oct] = Int32.Parse(hashtable[(int)Months.Oct].ToString()) + temp;
                                break;
                            };
                        case (int)Months.Nov:
                            {
                                var temp = isCalculateRevenue ? (decimal)month.Price : value;
                                hashtable[(int)Months.Nov] = Int32.Parse(hashtable[(int)Months.Nov].ToString()) + temp;
                                break;
                            };
                        case (int)Months.Dec:
                            {
                                var temp = isCalculateRevenue ? (decimal)month.Price : value;
                                hashtable[(int)Months.Dec] = Int32.Parse(hashtable[(int)Months.Dec].ToString()) + temp;
                                break;
                            };
                        default: break;
                    }
                }
                dictionary[item.Key] = hashtable;
            }
        }
    }
}