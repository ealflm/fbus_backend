using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Threading.Tasks;
using FBus.Data.Context;
using FBus.Data.Interfaces;
using FBus.Data.Models;

namespace FBus.Data.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly FBusContext _dbContext;

        public IGenericRepository<Admin> AdminRepository { get; }
        public IGenericRepository<Student> StudentRepository { get; }
        public IGenericRepository<Driver> DriverRepository { get; }
        public IGenericRepository<Station> StationRepository { get; }
        public IGenericRepository<BusVehicle> BusRepository { get; }
        public IGenericRepository<Route> RouteRepository { get; }
        public IGenericRepository<RouteStation> RouteStationRepository { get; }
        public IGenericRepository<Trip> TripRepository { get; }
        public IGenericRepository<StudentTrip> StudentTripRepository { get; }
        public IGenericRepository<DriverNotification> DriverNotificationRepository { get; }
        public IGenericRepository<StudentNotification> StudentNotificationRepository { get; }
        public IGenericRepository<Shift> ShiftRepository { get; }
        public IGenericRepository<TrackingLocation> TrackingLocationRepository { get; }

        public UnitOfWork(FBusContext dbContext,
                        IGenericRepository<Admin> adminRepository,
                        IGenericRepository<Student> studentRepository,
                        IGenericRepository<Driver> driverRepository,
                        IGenericRepository<Station> stationRepository,
                        IGenericRepository<BusVehicle> busRepository,
                        IGenericRepository<Route> routeRepository,
                        IGenericRepository<RouteStation> routeStationRepository,
                        IGenericRepository<Trip> tripRepository,
                        IGenericRepository<StudentTrip> studentTripRepository,
                        IGenericRepository<DriverNotification> driverNotificationRepository,
                        IGenericRepository<StudentNotification> studentNotificationRepository,
                        IGenericRepository<Shift> shiftRepository,
                        IGenericRepository<TrackingLocation> trackingLocationRepository
            )
        {
            _dbContext = dbContext;

            AdminRepository = adminRepository;
            StudentRepository = studentRepository;
            DriverRepository = driverRepository;
            StationRepository = stationRepository;
            BusRepository = busRepository;
            RouteRepository = routeRepository;
            RouteStationRepository = routeStationRepository;
            TripRepository = tripRepository;
            StudentTripRepository = studentTripRepository;
            DriverNotificationRepository = driverNotificationRepository;
            StudentNotificationRepository = studentNotificationRepository;
            ShiftRepository = shiftRepository;
            TrackingLocationRepository = trackingLocationRepository;
        }

        public FBusContext Context()
        {
            return _dbContext;
        }

        public DatabaseFacade Database()
        {
            return _dbContext.Database;
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}