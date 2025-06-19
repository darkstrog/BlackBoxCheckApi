using System.ComponentModel.DataAnnotations;

namespace BlackBoxCheckApi.Models
{
    public class ItemsList
    {
        public int Id { get; set; }
        public Guid IdGuid { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public bool IsShared { get; set; }
        public string? Description { get; set; }
        [DataType(DataType.Date)]
        public DateTime? CreatedAt { get; set; }
        [DataType(DataType.Date)]
        public DateTime? UpdateAt { get; set; }
        public List<BoxedItem> Items { get; set; } = new List<BoxedItem>();
    }
}
