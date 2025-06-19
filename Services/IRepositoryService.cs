namespace BlackBoxCheckApi.Services
{
    /// <summary>
    /// На случай обобщенного сервиса
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRepositoryService<T> where T : class
    {
        Task CreateAsync(T item);
        Task CreateAsync(IEnumerable<T> items);
        Task DeleteAsync(T item);
        IQueryable<T> GetAllAsync();
        Task UpdateAsync(T item);
        Task<T> GetByIdAsync(int id);
        Task<T> GetByGuidAsync(Guid id);

    }
}
