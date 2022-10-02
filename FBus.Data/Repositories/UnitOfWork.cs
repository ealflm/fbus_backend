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

        public UnitOfWork(FBusContext dbContext,
                        IGenericRepository<Admin> adminRepository
            )
        {
            _dbContext = dbContext;

            AdminRepository = adminRepository;
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