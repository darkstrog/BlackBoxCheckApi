using BlackBoxCheckApi.Models;
using System.ComponentModel.DataAnnotations;

namespace BlackBoxCheckApi.ApiModels.ResponseModels
{
    /// <summary>
    /// DTO с подробными данными о элементе листа
    /// </summary>
    public record BoxedItemDetailResponse
    {
        /// <summary>
        /// Guid элемента
        /// </summary>
        public Guid IdGuid { get; init; }
        /// <summary>
        /// Название предмета
        /// </summary>
        public string Name { get; init; }
        /// <summary>
        /// Количество 
        /// </summary>
        public string? Value { get; init; }
        /// <summary>
        /// Описание предмета
        /// </summary>
        public string? Description { get; init; }
        /// <summary>
        /// Дата добавления в базу
        /// </summary>
        [DataType(DataType.Date)]
        public DateTime? CreatedAt { get; init; }
        /// <summary>
        /// Дата изменения
        /// </summary>
        [DataType(DataType.Date)]
        public DateTime? UpdateAt { get; init; }
    }
}
