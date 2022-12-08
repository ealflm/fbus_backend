using Microsoft.EntityFrameworkCore;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FBus.Data.Interfaces
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        DbSet<TEntity> Query();

        Task<TEntity> GetById(Guid id);

        Task Add(TEntity entity);

        void Update(TEntity entity);

        Task Remove(Guid id);

        void Remove(TEntity entity);

        void UpdateFieldsChange(TEntity entity, params Expression<Func<TEntity, object>>[] includeProperties);
    }
}