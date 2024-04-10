using AutoMapper;
using Claims.Api.Services.Interfaces;
using Claims.Core.DTO.Requests;
using Claims.Domain.Constants;
using Claims.Presistence.Cosmos;

namespace Claims.Api.Services;

public class CoverService(
        IMapper mapper,
        ICosmosRepository<Cover> coverRepository)
    : ICoverService
{
    public async Task<IEnumerable<Cover>> GetAllCovers()
    {
        return await coverRepository.GetItemsAsync();
    }

    public async Task<Result<Cover, string>> CreateCover(CreateCoverRequest createCoverRequest)
    {
        if (createCoverRequest.EndDate > createCoverRequest.StartDate.AddYears(1))
        {
            return Result<Cover, string>.Err("Insurance period exceeds 1 year");
        }

        var cover = mapper.Map<Cover>(createCoverRequest);
        cover.Premium = ComputePremium(mapper.Map<PremiumRequest>(createCoverRequest));
        cover.Id = Guid.NewGuid();
        await coverRepository.AddItemAsync(cover);
        return Result<Cover, string>.Ok(cover);
    }

    public async Task<bool> DeleteCover(Guid id)
    {
        return await coverRepository.DeleteItemAsync(id);
    }

    public async Task<Cover?> GetCover(Guid id)
    {
        return await coverRepository.GetItemAsync(id);
    }

    public decimal ComputePremium(PremiumRequest premiumRequest)
    {
        var daysLeft = premiumRequest.EndDate.DayNumber - premiumRequest.StartDate.DayNumber + 1;
        var cost = 0m;
        var firstPeriod = Math.Min(daysLeft, 30) * CoverConstants.COVER_BASE_RATE *
                          CoverConstants.COVER_DISCOUNTS[(int)premiumRequest.Type][0];
        cost += firstPeriod;
        if (daysLeft <= 30)
        {
            return cost;
        }

        daysLeft -=30;
        var secondPeriod = Math.Min(daysLeft, 150) * CoverConstants.COVER_BASE_RATE *
                           CoverConstants.COVER_DISCOUNTS[(int)premiumRequest.Type][1];
        cost += secondPeriod;
        if (daysLeft <= 150)
        {
            return cost;
        }

        daysLeft -=150;
        cost += daysLeft * CoverConstants.COVER_BASE_RATE *
                CoverConstants.COVER_DISCOUNTS[(int)premiumRequest.Type][2];
        return cost;
    }
}