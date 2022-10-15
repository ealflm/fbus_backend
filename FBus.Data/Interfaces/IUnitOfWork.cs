using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Threading.Tasks;
using FBus.Data.Context;
using FBus.Data.Models;

namespace FBus.Data.Interfaces
{
    public interface IUnitOfWork
    {
        IGenericRepository<Admin> AdminRepository { get; }
        IGenericRepository<Student> StudentRepository { get; }
        IGenericRepository<Driver> DriverRepository { get; }
        IGenericRepository<Station> StationRepository { get; }
        IGenericRepository<BusVehicle> BusRepository { get; }
        IGenericRepository<Route> RouteRepository { get; }
        IGenericRepository<RouteStation> RouteStationRepository { get; }

        FBusContext Context();

        DatabaseFacade Database();

        Task SaveChangesAsync();
    }
}