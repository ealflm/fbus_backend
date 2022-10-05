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
        public UnitOfWork(FBusContext dbContext,
                        IGenericRepository<Admin> adminRepository,
                        IGenericRepository<Student> studentRepository,
                        IGenericRepository<Driver> driverRepository,
                        IGenericRepository<Station> stationRepository
            )
        {
            _dbContext = dbContext;

            AdminRepository = adminRepository;
            StudentRepository = studentRepository;
            DriverRepository = driverRepository;    
            StationRepository = stationRepository;
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