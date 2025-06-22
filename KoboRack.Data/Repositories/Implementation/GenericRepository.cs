using Microsoft.EntityFrameworkCore;
using KoboRack.Data.Context;
using KoboRack.Data.Repositories.Interface;
using System.Linq.Expressions;

namespace KoboRack.Data.Repositories.Implementation
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly SaviDbContext _context;

        public GenericRepository(SaviDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            await _context.SaveChangesAsync();
        }
        public T AddAsync2(T entity)
        {
            _context.Set<T>().Add(entity);
            _context.SaveChanges(); 
            return entity;
        }
        public async Task DeleteAllAsync(List<T> entities)
        {
            _context.Set<T>().RemoveRange(entities);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(T entity)
        {
            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> expression)
        {
            return await _context.Set<T>().AnyAsync(expression);
        }

        public List<T> FindAsync(Expression<Func<T, bool>> expression)
        {
            return _context.Set<T>().Where(expression).ToList();
        }

        public List<T> GetAll()
        {
            return _context.Set<T>().ToList();
        }

        public List<T> GetAll(Expression<Func<T, bool>> predicate = null)
        {
            if (predicate != null)
            {
                return _context.Set<T>().Where(predicate).ToList();
            }
            return _context.Set<T>().ToList();
        }

        public async Task<T> GetByIdAsync(string id)
        {
            return await _context.Set<T>().FindAsync(id);
        }
        public T GetById(string id)
        {
            return _context.Set<T>().Find(id);
        }

        public async Task<bool> CreateAsync(T entity)
        {

            await _context.Set<T>().AddAsync(entity);
            var saveTarget = await _context.SaveChangesAsync();
            if (saveTarget > 0)
            {
                return true;
            }
            return false;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void UpdateAsync(T entity)
        {
            _context.Set<T>().Update(entity);

        }     
    }
}
