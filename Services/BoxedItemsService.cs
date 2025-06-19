using BlackBoxCheckApi.ApiModels;
using BlackBoxCheckApi.Models;
using BlackBoxCheckApi.Models.Repository;
using Microsoft.EntityFrameworkCore;

namespace BlackBoxCheckApi.Services
{
    public class BoxedItemsService
    {
        private readonly IRepository<BoxedItem> _itemRepository;
        private readonly ILogger<ItemsListService> _logger;
        public BoxedItemsService(IRepository<BoxedItem> repo, ILogger<ItemsListService> logger)
        {
            _itemRepository = repo;
            _logger = logger;
        }
        public async Task<List<SearchResultResponse>> SearchItems(string searchString, Guid userId)
        {
            try
            {
                return await _itemRepository.GetAll()
                                              .Include(i => i.itemsList)
                                              .Where(i => i.itemsList.UserId == userId) // Фильтрация по userId
                                              .Where(p => EF.Functions.Like(p.Name, $"%{searchString}%") ||
                                                          EF.Functions.Like(p.Description, $"%{searchString}%"))
                                              .Select(z => new SearchResultResponse
                                              {
                                                  itemName = z.Name,
                                                  itemDescription = z.Description,
                                                  listName = z.itemsList.Name,
                                                  listId = z.itemsList.IdGuid.ToString(),
                                                  CreatedAt = z.itemsList.CreatedAt.ToString()
                                              })
                                              .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"{DateTime.Now.ToString("HH-mm-ss")} - Поиск в базе завершился ошибкой:", ex);
                throw;
            }
        }
        public async Task<BoxedItem> GetByIdAsync(int id)
        {
            try
            {
                return await _itemRepository.GetByIdAsync(id, o => o.itemsList);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{DateTime.Now.ToString("HH-mm-ss")} - Поиск в базе завершился ошибкой:", ex);
                throw;
            };
        }

        public async Task Delete(BoxedItem item)
        {
            try
            {
                await _itemRepository.DeleteAsync(item);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{DateTime.Now.ToString("HH-mm-ss")} - Поиск в базе завершился ошибкой:", ex);
                throw;
            };

        }
    }
}
