namespace BlackBoxCheckApi.ApiModels.RequestModels
{
    /// <summary>
    /// Модель для запроса на создание листа
    /// </summary>
    public class ItemsListCreateRequest
    {
        /// <summary>
        /// GUID объекта 
        /// </summary>
        public Guid? IdGuid { get; set; }
        public string Name { get; set; }
        public bool IsShared { get; set; }
        public string? Description { get; set; }
        public List<BoxedItemCreateRequest> Items { get; set; } = new List<BoxedItemCreateRequest>();
    }
    public class ItemsListCreateRequest1
    {
        private Guid? idGuid;
        public Guid? IdGuid 
        {
            get
            { 
                return idGuid; 
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                idGuid = value;
            }
        }
        public string Name { get; set; }
        public bool IsShared { get; set; }
        public string? Description { get; set; }
        public List<BoxedItemCreateRequest> Items { get; set; } = new List<BoxedItemCreateRequest>();
    }
}
