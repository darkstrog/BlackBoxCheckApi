using AutoMapper;
using BlackBoxCheckApi.ApiModels.RequestModels;
using BlackBoxCheckApi.ApiModels.ResponseModels;
using BlackBoxCheckApi.Models;

namespace BlackBoxCheckApi.Services
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ItemsList, ItemsListDetailResponse>();
            CreateMap<ItemsListCreateRequest, ItemsList>().ForMember(m => m.Items, o => o.Ignore());
            CreateMap<ItemsListUpdateRequest, ItemsList>().ForMember(m => m.Items, o => o.Ignore());

            CreateMap<BoxedItem, BoxedItemDetailResponse>();
            CreateMap<BoxedItemCreateRequest, BoxedItem>();
            CreateMap<BoxedItemUpdateRequest, BoxedItem>();

            CreateMap<ItemsListCreateRequest, ItemsListDetailResponse>().ForMember(m => m.Items, o => o.Ignore()); ;
            CreateMap<ItemsListDetailResponse, ItemsListCreateRequest>();

            CreateMap<BoxedItemCreateRequest, BoxedItemDetailResponse>();
            CreateMap<BoxedItemDetailResponse, BoxedItemCreateRequest>();
        }
    }
}
