using System.ComponentModel.DataAnnotations;

namespace BlackBoxCheckApi.ApiModels.ResponseModels
{
    public class ItemsListDetailResponse
    {
        public Guid IdGuid { get; set; }
        public string Name { get; set; }
        public bool IsShared { get; set; }
        public string? Description { get; set; }

        [DataType(DataType.Date)]
        public DateTime? CreatedAt { get; set; }

        [DataType(DataType.Date)]
        public DateTime? UpdateAt { get; set; }
        public List<BoxedItemDetailResponse> Items { get; set; } = new List<BoxedItemDetailResponse>();
    }
}
