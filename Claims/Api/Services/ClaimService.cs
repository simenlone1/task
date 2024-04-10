using AutoMapper;
using Claims.Api.Services.Interfaces;
using Claims.Core.DTO.Requests;
using Claims.Presistence.Cosmos;

namespace Claims.Api.Services;

public class ClaimService(
        ICosmosRepository<Claim> claimRepository, 
        IMapper mapper,
        ICosmosRepository<Cover> coverRepository)
    : IClaimService
{
    public async Task<IEnumerable<Claim>> GetAllClaims()
    {
        return await claimRepository.GetItemsAsync();
    }

    public async Task<Result<Claim, string>> CreateClaim(CreateClaimRequest createClaimRequest)
    {
        var cover = await coverRepository.GetItemAsync(Guid.Parse(createClaimRequest.CoverId));
        if (cover == null)
        {
            return Result<Claim, string>.Err($"The requested cover does not exist: {createClaimRequest.CoverId}");
        }
        
        var timeZoneAdjusted = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(createClaimRequest.Created, "GMT Standard Time");
        var dateTimeOnly = new DateOnly(timeZoneAdjusted.Year, timeZoneAdjusted.Month, timeZoneAdjusted.Day);
        
        if (dateTimeOnly >= cover.StartDate && dateTimeOnly <= cover.EndDate)
        {
            //This leaks the validity period of the cover.
            return Result<Claim, string>.Err($"The requested time is not range between: {cover.StartDate} and {cover.EndDate}");
        }
        var claim = mapper.Map<Claim>(createClaimRequest);
        claim.Id = Guid.NewGuid();
        await claimRepository.AddItemAsync(claim);
        return claim;
    }

    public async Task<bool> DeleteClaim(Guid id)
    {
        return await claimRepository.DeleteItemAsync(id);
    }

    public async Task<Claim?> GetClaim(Guid id)
    {
        return await claimRepository.GetItemAsync(id);
    }
}