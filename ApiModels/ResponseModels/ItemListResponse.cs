using BlackBoxCheckApi.ApiModels.ResponseModels;

namespace BlackBoxCheckApi.ApiModels
{

    public record ItemListResponse
    {
        public ItemsListDetailResponse ItemsList { get; init; } //= new();
        public string QRbitmap { get; init; }

    }
}
