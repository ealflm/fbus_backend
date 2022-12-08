using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FBus.Business.BaseBusiness.Configuration;
using FBus.Business.BaseBusiness.ViewModel;
using FBus.Business.TripManagement.Interfaces;
using FBus.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace TourismSmartTransportation.API
{
    public class FbusBackgroundService : BackgroundService
    {
        private readonly ILogger<FbusBackgroundService> _logger;
        private readonly IServiceScopeFactory _serviceProvider;

        public FbusBackgroundService(ILogger<FbusBackgroundService> logger, IServiceScopeFactory serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    _logger.LogInformation("Start FbusBackgroundService: ExecuteAsync");

                    // Inject service
                    var _serviceScope = scope.ServiceProvider.GetRequiredService<ITripManagementService>();

                    // Get Current Date time
                    var currentDate = DateTime.UtcNow.AddHours(7);
                    var dateCheck = currentDate.Date;
                    var timeCheck = currentDate.TimeOfDay;

                    // Get Trips to update status
                    var trips = await _serviceScope.UnitOfWork.TripRepository
                                .Query()
                                .Where(x => x.Status == (int)TripStatus.Waiting || x.Status == (int)TripStatus.Active)
                                .Select(x => new
                                {
                                    TripId = x.TripId,
                                    DriverId = x.DriverId.Value,
                                    BusId = x.BusVehicleId,
                                    Date = x.Date,
                                    TimeStart = x.TimeStart,
                                    TimeEnd = x.TimeEnd,
                                    Status = x.Status
                                })
                                .ToListAsync();

                    foreach (var trip in trips)
                    {
                        if (dateCheck.CompareTo(trip.Date) == 0 &&
                            trip.TimeEnd.CompareTo(timeCheck) <= 0)
                        {
                            var updatedTrip = new Trip() { TripId = trip.TripId };

                            if (trip.Status == (int)TripStatus.Active)
                            {
                                updatedTrip.Status = (int)TripStatus.Done;
                                var driver = await _serviceScope.UnitOfWork.DriverRepository.GetById(trip.DriverId);
                                var bus = await _serviceScope.UnitOfWork.BusRepository.GetById(trip.BusId);
                                var tripFuture = await _serviceScope.UnitOfWork.TripRepository
                                                    .Query()
                                                    .Where(x =>
                                                        (x.Date.CompareTo(dateCheck) >= 0) &&
                                                        x.Status == (int)TripStatus.Waiting &&
                                                        (x.TimeStart.CompareTo(timeCheck) >= 0) &&
                                                        x.DriverId != null &&
                                                        x.DriverId.Value == driver.DriverId
                                                    )
                                                    .FirstOrDefaultAsync();

                                driver.Status = tripFuture != null ? (int)DriverStatus.Assigned : (int)DriverStatus.Active;
                                bus.Status = (int)BusStatus.Active;

                                _serviceScope.UnitOfWork.DriverRepository.Update(driver);
                                _serviceScope.UnitOfWork.BusRepository.Update(bus);
                            }
                            else
                            {
                                updatedTrip.Status = (int)TripStatus.NotUsed;
                            }

                            _serviceScope.UnitOfWork.TripRepository.UpdateFieldsChange(updatedTrip, x => x.Status);
                        }
                    }

                    await _serviceScope.UnitOfWork.SaveChangesAsync();

                    // Interval in specific time
                    await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
                }

            }
            _logger.LogInformation("End FbusBackgroundService: ExecuteAsync");
        }
    }
}