using System.ComponentModel.DataAnnotations;

namespace BlackBoxCheckApi.ApiModels.RequestModels
{
    /// <summary>
    /// Поисковый запрос
    /// </summary>
    public class SearchRequest
    {
        /// <summary>
        /// Искомый текст
        /// </summary>
        [Required]
        public string Text { get; set; } = default!;
    }
}
