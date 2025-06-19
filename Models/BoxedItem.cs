using System.ComponentModel.DataAnnotations;

namespace BlackBoxCheckApi.Models
{
    public class BoxedItem
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public Guid IdGuid { get; set; }
        public string Name { get; set; }
        public string? Value { get; set; }
        public string? Description { get; set; }
        [DataType(DataType.Date)]
        public DateTime? CreatedAt { get; set; }
        [DataType(DataType.Date)]
        public DateTime? UpdateAt { get; set; }
        public ItemsList itemsList { get; set; }
    }
}
