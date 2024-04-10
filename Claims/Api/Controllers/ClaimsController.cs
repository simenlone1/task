using AutoMapper;
using Claims.Api.Services.Interfaces;
using Claims.Core.DTO.Requests;
using Claims.Core.DTO.Response;
using Claims.Persistence.ServiceBus.Interface;
using Microsoft.AspNetCore.Mvc;

namespace Claims.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClaimsController(ILogger<ClaimsController> logger, IAuditRepository auditer,
            IClaimService claimService, IMapper mapper)
        : ControllerBase
    {
        //_cosmosDbService = cosmosDbService;

        /// <summary>
        /// Returns a list of all claims
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<ClaimResponse>> GetAsync()
        {
            var claims = await claimService.GetAllClaims();
            return mapper.Map<IEnumerable<ClaimResponse>>(claims);
        }

        /// <summary>
        /// Creates a new Claim
        /// </summary>
        /// <param name="claimRequest"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<ClaimResponse>> CreateAsync(CreateClaimRequest claimRequest)
        {
            var claim = await claimService.CreateClaim(claimRequest);
            var response = claim.Match<ActionResult>(async claim =>
                {
                    await auditer.AuditClaim(claim.Id, "POST");
                    return Ok(mapper.Map<ClaimResponse>(claim));
                }
                , err => Task.FromResult<ActionResult>(BadRequest(err))
            );
            return await response;
        }

        /// <summary>
        /// Deletes a claim by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAsync(Guid id)
        {
            //TODO: check bool should be 204
            await auditer.AuditClaim(id, "DELETE");
            if(await claimService.DeleteClaim(id)) return NoContent();
            return NotFound();
        }

        /// <summary>
        /// Returns a claim by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<ClaimResponse>> GetAsync(Guid id)
        {
            var claim = await claimService.GetClaim(id);
            if (claim is null) return NotFound();
            return Ok(mapper.Map<ClaimResponse>(claim));
        }
    }
}