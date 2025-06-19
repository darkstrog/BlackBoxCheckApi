namespace BlackBoxCheckApi.ApiModels
{
    /// <summary>
    /// DTO для результатов поиска
    /// </summary>
    public record SearchResultResponse
    {
        /// <summary>
        /// Имя листа во вложенной коллекции которого найден искомый текст
        /// </summary>
        public string listName { get; init; }
        /// <summary>
        /// Дата создания листа
        /// </summary>
        public string CreatedAt { get; init; }
        /// <summary>
        /// Непосредственно элемент в котором найдено совпадение с искомой строкой
        /// </summary>
        public string itemName { get; init; }
        /// <summary>
        /// Описание элемента
        /// </summary>
        public string itemDescription { get; init; }
        /// <summary>
        /// Guid листа
        /// </summary>
        public string listId { get; init; }
    }
}
