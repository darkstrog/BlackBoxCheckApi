using AutoMapper;
using BlackBoxCheckApi.ApiModels;
using BlackBoxCheckApi.ApiModels.RequestModels;
using BlackBoxCheckApi.ApiModels.ResponseModels;
using BlackBoxCheckApi.Models;
using BlackBoxCheckApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlackBoxCheckApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/v1/[controller]")]
    public class ItemsListController : ControllerBase
    {
        private readonly IQRService _qrService;
        private readonly BoxedItemsService _boxedItemsService;
        private readonly ItemsListService _itemsListService;
        private readonly IConfiguration _config;
        private readonly ILogger<ItemsListController> _logger;
        private readonly IMapper _mapper;
        private readonly string _url;
        public ItemsListController(IQRService qRService, BoxedItemsService boxedItemsService,
                                       ItemsListService itemsListService, IConfiguration configuration,
                                       ILogger<ItemsListController> logger, IMapper mapper)
        {
            _qrService = qRService;
            _boxedItemsService = boxedItemsService;
            _itemsListService = itemsListService;
            _config = configuration;
            _logger = logger;
            _mapper = mapper;
            _url = _config["Options:QRLinkAddress"] ?? "https://" + HttpContext.Request.Host.Value;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="listGuid"></param>
        /// <returns>Список предметов</returns>
        /// <response code="200">Возвращает запрошенный список</response>
        /// <response code="404">Если список не найден</response>
        [HttpGet("Getbyid/{listGuid}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetListByGuid(Guid listGuid)
        {
            try
            {
                Guid userId;
                Guid.TryParse(User.FindFirst("UserId")?.Value, out userId);
                _logger.LogInformation($"Запрошен guid: {listGuid} пользователем: {userId}");

                var itemsList = await _itemsListService.GetListById(listGuid, userId);


                if (itemsList == null)
                {
                    _logger.LogInformation($"Список с ID: {listGuid}, не найден");
                    return NotFound();
                }
                
                _logger.LogInformation($"{itemsList?.IdGuid} Успешно получен");
                string linkToItem = _url + $"/GetList/{listGuid}";
                var qrmap = _qrService.GetBase64PngQRCode(linkToItem);
                var model = new ItemListResponse
                {
                    ItemsList = _mapper.Map(itemsList, new ItemsListDetailResponse()),
                    QRbitmap = qrmap
                };
                return Ok(model);
            }
            catch (Exception)
            {
                _logger.LogError($"Не удалось получить данные с id: {listGuid}");
                return StatusCode(500, "Internal server error.");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Возвращает ссылку на список</returns>
        /// <response code="401">Для добавления списка требуется авторизация</response>
        /// <response code="201">Лист добавлен</response>
        [HttpPost("Add")]
        public async Task<IActionResult> AddList([FromBody] ItemsListCreateRequest request)
        {
            Guid userId;
            _logger.LogInformation($"{DateTime.Now.ToString("HH-mm-ss")} - id текущего пользователя");

            if (Guid.TryParse(User.FindFirst("UserId")?.Value, out userId))
            {
                var requestGuid = request.IdGuid ?? Guid.NewGuid();
                request.IdGuid = requestGuid;
                await _itemsListService.Add(request, userId);
                
                string linkToItem = _url + $"/GetList/{request.IdGuid}";
                return Created(linkToItem,null);
            }
            return Unauthorized();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Возвращает список совпадений в предметах. Каждое совпадение вложено в свой лист</returns>
        /// <response code="401">Для добавления списка требуется авторизация</response>
        [HttpPost("Search")]
        [ProducesResponseType(typeof(List<SearchResultResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Search([FromBody]SearchRequest request)
        {
            if (string.IsNullOrEmpty(request.Text))
                return BadRequest();
            Guid userId;
            _logger.LogInformation($"{DateTime.Now.ToString("HH-mm-ss")} - id текущего пользователя");

            if (Guid.TryParse(User.FindFirst("UserId")?.Value, out userId))
            {
                _logger.LogInformation($"{DateTime.Now.ToString("HH-mm-ss")} - Попытка спарсить guid: {userId}");
                List<SearchResultResponse> result = await _boxedItemsService.SearchItems(request.Text, userId);
                return Ok(result);
            }
            return Unauthorized();
        }

        [HttpDelete("Delete/{listGuid}")]
        public async Task<IActionResult> DeleteList(Guid listGuid)
        {
            Guid userId;
            if (!Guid.TryParse(User.FindFirst("UserId")?.Value, out userId))
            {
                _logger.LogError("Не валидный UserId в token");
                return StatusCode(500);
            }
            try
            {
                await _itemsListService.Delete(listGuid, userId);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500);
            }
            
            return Ok();
        }
        [HttpPost("Update")]
        public async Task<IActionResult> Update([FromBody] ItemsListUpdateRequest request)
        {
            Guid userId;
            if (!Guid.TryParse(User.FindFirst("UserId")?.Value, out userId))
            {
                _logger.LogError("Не валидный UserId в token");
                return StatusCode(500);
            }
            try
            {
                await _itemsListService.Update(request, userId);
                return Ok(request.IdGuid);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "При обновлении произошла ошибка");
                return StatusCode(500, "Произошла внутренняя ошибка");
            }
        }
    }
}
