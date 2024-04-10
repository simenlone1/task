using AutoMapper;
using Claims.Core.DTO.Requests;
using Claims.Core.DTO.Response;

namespace Claims.Core.AutoMapper;

public class CoverProfile : Profile
{
    public CoverProfile()
    {
        CreateMap<Cover, CoverResponse>();
        CreateMap<CreateCoverRequest, Cover>();
        CreateMap<CreateCoverRequest, PremiumRequest>();
    }
}
