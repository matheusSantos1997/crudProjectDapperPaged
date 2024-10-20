using Microsoft.EntityFrameworkCore;

namespace crudDapperWithEf.API.Repositories.Interfaces
{
    public interface IGenericRepository
    {
        void Add<T>(T entity) where T : class;

        void Update<T>(T entity) where T : class;

        void Attach<T>(T entity, EntityState state) where T : class;

        void Delete<T>(T entity) where T : class;

        Task<bool> CommitAsync();
    }
}