using AutoMapper;
using BlackBoxCheckApi.ApiModels.RequestModels;
using BlackBoxCheckApi.Models;
using BlackBoxCheckApi.Models.Repository;
using Microsoft.EntityFrameworkCore;

namespace BlackBoxCheckApi.Services
{
    public class ItemsListService
    {
        private readonly IRepository<ItemsList> _itemsListRepository;
        private readonly ILogger<ItemsListService> _logger;
        private readonly IMapper _mapper;
        public ItemsListService(IRepository<ItemsList> repo, ILogger<ItemsListService> logger, IMapper mapper)
        {
            _itemsListRepository = repo;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<List<ItemsList>> GetLastRecords(int count, Guid userId)
        {
            if (count <= 0)
            {
                throw new ArgumentOutOfRangeException("count");
            }
            else
            {
                try
                {
                    _logger.LogInformation("Получаем последние добавленные в базу записи");
                    return await _itemsListRepository.GetAll()
                                                     .Where(u => u.UserId == userId)
                                                     .OrderByDescending(x => x.Id)
                                                     .Take(count)
                                                     .ToListAsync();
                }
                catch (Exception)
                {
                    _logger.LogError("Ошибка при получении последних записей");
                    throw;
                }


            }
        }
        /// <summary>
        /// Возвращает лист с ID полученным через параметры Если лист найден и он публичный то он вернется из метода.
        /// Если лист приватный то метод сверяет переданный userId с id владельца листа и возвращает лист в случае совпадения.
        /// Если лист приватный а userId не передан или не совпал с владельцем вернется null.
        /// </summary>
        /// <param name="listGuid">Guid искомого листа</param>
        /// <param name="userGuid">Guid пользователя запросившего лист, по умолчанию используется нулевой Guid</param>
        /// <returns></returns>
        public async Task<ItemsList?> GetListById(Guid listGuid, Guid userGuid = default)
        {
            try
            {
                var itemsList = await _itemsListRepository.GetByGuidAsync(listGuid, o => o.Items);
                _logger.LogInformation($"Список успешно получен");

                if (itemsList == null) return null;
                if (itemsList.IsShared || userGuid == itemsList.UserId)
                {
                    _logger.LogInformation("Запрошенные данные получены успешно.");
                    return itemsList;
                }
                _logger.LogError($"Не удалось получить данные так как они приватны и пренадлежат другому пользователю, id записи: {listGuid} userid запросившего: {userGuid}");
                return null;
            }
            catch (Exception)
            {
                _logger.LogError($"Не удалось получить данные {listGuid}");
                throw;
            }
        }
        public async Task Add(ItemsListCreateRequest dtoItemsList, Guid userGuid)
        {
            try
            {
                var itemsList = _mapper.Map<ItemsList>(dtoItemsList);
                itemsList.CreatedAt = DateTime.Now;
                itemsList.UpdateAt = DateTime.Now;
                itemsList.UserId = userGuid;
                foreach(var item in dtoItemsList.Items)
                {
                    item.IdGuid = item.IdGuid ?? Guid.NewGuid();
                }
                itemsList.Items = _mapper.Map<List<BoxedItem>>(dtoItemsList.Items);

                foreach (BoxedItem item in itemsList.Items)
                {
                    item.IdGuid = Guid.NewGuid();
                    item.CreatedAt = DateTime.Now;
                    item.UpdateAt = DateTime.Now;
                    item.UserId = userGuid;
                }

                await _itemsListRepository.CreateAsync(itemsList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Не удалось добавить данные");
                throw;
            }
        }

        public async Task Delete(Guid listGuid, Guid userGuid)
        {
            try
            {
                var existingItem = await _itemsListRepository.GetByGuidAsync(listGuid);
                if (existingItem == null || userGuid != existingItem.UserId)
                {
                    _logger.LogInformation($"{listGuid} не найден или не принадлежит инициатору запроса: ({userGuid}) и не может быть удален");
                    throw new UnauthorizedAccessException();
                }

                await _itemsListRepository.DeleteAsync(existingItem);
                _logger.LogInformation($"{listGuid} удален");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Не удалось удалить данные: {listGuid}");
                throw;
            }
        }

        public async Task Update(ItemsListUpdateRequest dtoListUpdate, Guid userGuid)
        {
            try
            {
                var existingItem = await _itemsListRepository.GetByGuidAsync(dtoListUpdate.IdGuid, o => o.Items);
                if (existingItem == null || existingItem.UserId != userGuid)
                {
                    _logger.LogInformation($"{dtoListUpdate.IdGuid} не найден или не принадлежит инициатору запроса ({userGuid}) и не может быть обновлен");
                    return;
                }
                //мапим dto в сущность ef
                var listUpdate = _mapper.Map<ItemsList>(dtoListUpdate);
                listUpdate.CreatedAt = existingItem.CreatedAt;
                listUpdate.UpdateAt = DateTime.Now;
                listUpdate.UserId = userGuid;
                listUpdate.Id = existingItem.Id;
                //мапим вхождения
                listUpdate.Items = _mapper.Map<List<BoxedItem>>(dtoListUpdate.Items);

                foreach (BoxedItem item in listUpdate.Items)
                {
                    item.UpdateAt = DateTime.Now;
                    //если это вновь добавленный BoxedItem то с клиента он может прилететь без guid
                    //поэтому присваеваем здесь его
                    if (item.IdGuid == Guid.Empty) { item.IdGuid = Guid.NewGuid(); }
                    //так как dto не содержит id пользователя здесь мы его присваиваем
                    //если это существующий BoxedItem то в репозитории при сравнении он совпадет с стем что базе
                    item.UserId = userGuid;
                }
                await _itemsListRepository.UpdateAsync(existingItem, listUpdate);
                _logger.LogInformation($"{listUpdate.IdGuid} обновлен");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Не удалось обновить данные: {dtoListUpdate.IdGuid} ");
                throw;
            }

        }
    }
}
