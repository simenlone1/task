using Claims.Core.DTO.Requests;

namespace Claims.Api.Services.Interfaces;

public interface ICoverService
{
    Task<IEnumerable<Cover>> GetAllCovers();
    Task<Result<Cover, string>> CreateCover(CreateCoverRequest createCoverRequest);
    Task<bool> DeleteCover(Guid id);
    Task<Cover?> GetCover(Guid id);
    decimal ComputePremium(PremiumRequest premiumRequest);
}