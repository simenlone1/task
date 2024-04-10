using Claims.Core.DTO.Requests;
using Claims.Core.DTO.Response;

namespace Claims.Api.Services.Interfaces;

public interface IClaimService
{
    Task<IEnumerable<Claim>> GetAllClaims();
    Task<Result<Claim, string>> CreateClaim(CreateClaimRequest createClaimRequest);
    Task<bool> DeleteClaim(Guid id);
    Task<Claim?> GetClaim(Guid id);
}