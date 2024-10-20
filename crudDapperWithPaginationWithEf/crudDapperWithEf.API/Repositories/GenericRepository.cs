using crudDapperWithEf.API.Context;
using crudDapperWithEf.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace crudDapperWithEf.API.Repositories
{
    public class GenericRepository : IGenericRepository
    {
        private readonly DataContext _context;

        public GenericRepository(DataContext context)
        {
            _context = context;
        }

        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }

        public void Update<T>(T entity) where T : class
        {
            _context.Update(entity);
        }

        public void Attach<T>(T entity, EntityState state) where T : class
        {
            _context.Attach(entity);
            _context.Entry(entity).State = state;
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }

        public async Task<bool> CommitAsync()
        {
            bool result = await _context.SaveChangesAsync() > 0;
            return result;
        }
    }
}