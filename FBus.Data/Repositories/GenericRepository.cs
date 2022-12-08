using Microsoft.EntityFrameworkCore;
using System;
using System.Reflection;
using System.Threading.Tasks;
using FBus.Data.Context;
using FBus.Data.Interfaces;
using System.Linq.Expressions;

namespace FBus.Data.Repositories
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        private readonly DbSet<TEntity> _dbSet;
        private readonly FBusContext _dbContext;

        public GenericRepository(FBusContext dbContext)
        {
            _dbSet = dbContext.Set<TEntity>();
            _dbContext = dbContext;
        }

        public DbSet<TEntity> Query()
        {
            return _dbSet;
        }

        public async Task<TEntity> GetById(Guid id)
        {
            var data = await _dbSet.FindAsync(id);
            return data;
        }

        public async Task Add(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void Update(TEntity entity)
        {
            _dbSet.Attach(entity);
            string name = entity.GetType().Name;
            foreach (PropertyInfo prop in entity.GetType().GetProperties())
            {
                if (prop.GetGetMethod().IsVirtual) continue;
                if (prop.Name.Equals(name + "Id")) continue;
                if (prop.Name.Contains("Id"))
                {
                    if (name.Contains(prop.Name.Substring(0, prop.Name.Length - 2))) continue;
                };
                if (prop.GetValue(entity, null) != null)
                {
                    _dbContext.Entry(entity).Property(prop.Name).IsModified = true;
                }
            }
        }

        public async Task Remove(Guid id)
        {
            var entity = await GetById(id);
            _dbSet.Remove(entity);
        }

        public void Remove(TEntity entity)
        {
            _dbSet.Remove(entity);
        }

        public void UpdateFieldsChange(TEntity entity, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            _dbSet.Attach(entity);
            foreach (var prop in includeProperties)
            {
                _dbContext.Entry(entity).Property(prop).IsModified = true;
            }
        }
    }
}