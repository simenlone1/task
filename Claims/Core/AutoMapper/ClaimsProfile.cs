using AutoMapper;
using Claims.Core.DTO.Requests;
using Claims.Core.DTO.Response;

namespace Claims.Core.AutoMapper;

public class ClaimsProfile: Profile
{
    public ClaimsProfile()
    {
        CreateMap<Claim, ClaimResponse>();
        CreateMap<CreateClaimRequest, Claim>();
    }
    
}