using AutoMapper;
using Claims.Api.Services.Interfaces;
using Claims.Core.DTO.Requests;
using Claims.Core.DTO.Response;
using Claims.Persistence.ServiceBus.Interface;
using Microsoft.AspNetCore.Mvc;

namespace Claims.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class CoversController(IMapper mapper, IAuditRepository auditer, ILogger<CoversController> logger,
        ICoverService coverService)
    : ControllerBase
{
    /// <summary>
    /// Computes a cover's premium
    /// </summary>
    /// <param name="startDate"></param>
    /// <param name="endDate"></param>
    /// <param name="coverType"></param>
    /// <returns></returns>
    [HttpPost("ComputePremium")]
    public async Task<ActionResult> ComputePremiumAsync(PremiumRequest premiumRequest)
    {
        var premium = coverService.ComputePremium(premiumRequest);
        return Ok(premium);
    }

    /// <summary>
    /// Returns a list of all covers
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IEnumerable<CoverResponse>> GetAsync()
    {
        var covers = await coverService.GetAllCovers();
        return mapper.Map<IEnumerable<CoverResponse>>(covers);
    }

    /// <summary>
    /// Returns a cover by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<Cover>> GetAsync(Guid id)
    {
        var cover = await coverService.GetCover(id);
        if (cover is null) return NotFound();
        return Ok(mapper.Map<CoverResponse>(cover));
    }

    /// <summary>
    /// Creates a cover
    /// </summary>
    /// <param name="coverRequest"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult> CreateAsync(CreateCoverRequest coverRequest)
    {
        var cover = await coverService.CreateCover(coverRequest);
        var response = cover.Match<ActionResult>(async cover =>
            {
                await auditer.AuditCover(cover.Id, "POST");
                return Ok(mapper.Map<CoverResponse>(cover));
            }
            , err => Task.FromResult<ActionResult>(BadRequest(err))
        );
        return await response;
    }

    /// <summary>
    /// Delete a cover by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAsync(Guid id)
    {
        await auditer.AuditCover(id, "DELETE");
        if (await coverService.DeleteCover(id)) return NoContent();
        return NotFound();
    }
}