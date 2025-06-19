using System.ComponentModel.DataAnnotations;

namespace BlackBoxCheckApi.ApiModels.RequestModels
{
    public class GetListByGuidRequest
    {
        [Required]
        public string _guid { get; set; }
        [Required]
        public string _userGuid { get; set; }
    }
}
