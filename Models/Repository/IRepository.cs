using System.Linq.Expressions;

namespace BlackBoxCheckApi.Models.Repository
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetSomeItemsAsync(int startId, int count);
        IQueryable<T> GetAll();
        Task<T> GetByIdAsync(int id, params Expression<Func<T, object>>[] includes);
        Task<T> GetByGuidAsync(Guid id, params Expression<Func<T, object>>[] includes);
        Task CreateAsync(T entity);
        Task CreateRangeAsync(IEnumerable<T> entity);
        Task UpdateAsync(T existingEntity, T entity);
        Task DeleteAsync(T entity);
    }
}
