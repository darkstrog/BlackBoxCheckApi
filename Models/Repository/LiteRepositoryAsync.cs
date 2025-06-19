using BlackBoxCheckApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BlackBoxCheckApi.Models.Repository
{
    public class LiteRepositoryAsync<T> : IRepository<T> where T : class
    {
        private readonly DbContext _context;
        private readonly DbSet<T> _dbSet;
        //используем BlackBoxDbContext вместо DbContext так как именно наследуемый класс зарегистрирован в service
        //AddDbContext<BlackBoxDbContext>
        //для использования базового DbContext контекст нужно регистрировать явно через AddScoped(<DbContext>,<BlackBoxDbContext>)
        public LiteRepositoryAsync(BlackBoxDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();

        }
        public async Task CreateAsync(T item)
        {
            await _dbSet.AddAsync(item);
            await _context.SaveChangesAsync();
        }
        public async Task CreateRangeAsync(IEnumerable<T> items)
        {
            await _dbSet.AddRangeAsync(items);
            await _context.SaveChangesAsync();
        }
        //public async Task DeleteAsync(Guid id)
        //{
        //    var item = await _dbSet.FindAsync(id);

        //    if (item != null)
        //    {
        //        _dbSet.Remove(item);
        //        await _context.SaveChangesAsync();
        //    }
        //}
        public async Task DeleteAsync(T item)
        {
            _dbSet.Remove(item);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(T existingItem, T item)
        {
            _context.Entry(existingItem).CurrentValues.SetValues(item);
            if (typeof(T) == typeof(ItemsList))
            {
                var existingList = existingItem as ItemsList;
                var updateList = item as ItemsList;
                if (existingList == null || updateList == null)
                {
                    throw new ArgumentException("Объекты не являются ItemsList или равны null.");
                }

                if (existingList.Items == null)
                {
                    throw new InvalidOperationException("existingList.Items не может быть null.");
                }
                UpdateBoxedItems(existingList.Items, updateList.Items ?? new List<BoxedItem>());
            }
            await _context.SaveChangesAsync();
        }
        private void UpdateBoxedItems(ICollection<BoxedItem> existingItems, ICollection<BoxedItem> updatedItems)
        {
            // Удаляем BoxedItem, которых нет в updatedItems
            foreach (var existingItem in existingItems.ToList())
            {
                if (!updatedItems.Any(c => c.IdGuid == existingItem.IdGuid))
                {
                    existingItems.Remove(existingItem);
                }
            }

            // Обновляем или добавляем BoxedItem
            foreach (var updatedItem in updatedItems)
            {
                var existingItem = existingItems.FirstOrDefault(c => c.IdGuid == updatedItem.IdGuid);
                if (existingItem != null)
                {
                    // Копируем свойства из updatedItem в existingItem
                    updatedItem.Id = existingItem.Id;
                    updatedItem.CreatedAt = existingItem.CreatedAt;
                    _context.Entry(existingItem).CurrentValues.SetValues(updatedItem);
                }
                else
                {
                    // Добавляем новый BoxedItem
                    updatedItem.CreatedAt = DateTime.Now;
                    updatedItem.UpdateAt = DateTime.Now;
                    existingItems.Add(updatedItem);
                }
            }
        }
        public async Task<T> GetByGuidAsync(Guid id, params Expression<Func<T, object>>[] includes)
        {
            var query = _dbSet.AsQueryable();

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.FirstOrDefaultAsync(e => EF.Property<Guid>(e, "IdGuid").Equals(id));
        }

        public IQueryable<T> GetAll()
        {
            return _dbSet;
        }

        public async Task<IEnumerable<T>> GetSomeItemsAsync(int startId, int count)
        {
            return await _dbSet.Skip(startId).Take(count).ToListAsync();
        }

        //public async Task UpdateAsync(Guid itemId, T item)
        //{
        //    var existingItem = await _dbSet.FindAsync(itemId);

        //    if (existingItem != null)
        //    {
        //        _context.Entry(existingItem).CurrentValues.SetValues(item);
        //        await _context.SaveChangesAsync();
        //    }
        //}


        public async Task<T> GetByIdAsync(int id, params Expression<Func<T, object>>[] includes)
        {
            var query = _dbSet.AsQueryable();

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.FirstOrDefaultAsync(e => EF.Property<int>(e, "Id").Equals(id));
        }
    }
}
