namespace BlackBoxCheckApi.ApiModels.RequestModels
{
    public class BoxedItemCreateRequest
    {
        public Guid? IdGuid { get; set; } //= Guid.NewGuid();
        public string Name { get; set; }
        public string? Value { get; set; }
        public string? Description { get; set; }

        //public ItemsList itemsList { get; set; }
    }
}
