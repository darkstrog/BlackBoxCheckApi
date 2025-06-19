using BlackBoxCheckApi.Models.Profiles;

namespace BlackBoxCheckApi.ApiModels.ResponseModels
{
    public class VerifyUserResponse
    {
        public bool IsVerified { get; init; }
        public UserProfile? UserProfile { get; init; }
        public string? ErrorMessage { get; init; }
    }
}
