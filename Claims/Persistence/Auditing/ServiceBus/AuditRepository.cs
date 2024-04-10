using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Claims.Auditing;
using Claims.Persistence.ServiceBus.Interface;

namespace Claims.Persistence.ServiceBus;

public class AuditRepository(ServiceBusSender sender) : IAuditRepository
{
    public async Task AuditClaim(Guid id, string httpRequestType)
    {
        var claimAudit = new ClaimAudit()
        {
            Created = DateTimeOffset.UtcNow,
            HttpRequestType = httpRequestType,
            ClaimId = id.ToString()
        };
        
        var json = JsonSerializer.Serialize(claimAudit);
        var message = new ServiceBusMessage(json);

        await sender.SendMessageAsync(message);
    }

    public async Task AuditCover(Guid id, string httpRequestType)
    {
        var coverAudit = new CoverAudit()
        {
            Created = DateTimeOffset.UtcNow,
            HttpRequestType = httpRequestType,
            CoverId = id.ToString()
        };
        
        var json = JsonSerializer.Serialize(coverAudit);
        var message = new ServiceBusMessage(json);

        await sender.SendMessageAsync(message);
    }
}