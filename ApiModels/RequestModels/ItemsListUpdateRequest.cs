using System.ComponentModel.DataAnnotations;

namespace BlackBoxCheckApi.ApiModels.RequestModels
{
    public class ItemsListUpdateRequest
    {
        /// <summary>
        /// GUID объекта 
        /// </summary>
        [Required]
        public Guid IdGuid { get; set; }
        public string? Name { get; set; }
        public bool IsShared { get; set; }
        public string? Description { get; set; }
        public List<BoxedItemUpdateRequest> Items { get; set; } = new List<BoxedItemUpdateRequest>();
        
    }
}
