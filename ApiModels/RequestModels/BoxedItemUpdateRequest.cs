using System.ComponentModel.DataAnnotations;

namespace BlackBoxCheckApi.ApiModels.RequestModels
{
    /// <summary>
    /// DTO для обновления элементов коллекции листа
    /// </summary>
    public class BoxedItemUpdateRequest
    {
        /// <summary>
        /// Guid обновляемого элемента
        /// </summary>
        [Required]
        public Guid IdGuid { get; set; }
        public string Name { get; set; }
        public string? Value { get; set; }
        public string? Description { get; set; }
    }
}
