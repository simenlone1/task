namespace Claims.Persistence.ServiceBus.Interface;

public interface IAuditRepository
{
    public Task AuditClaim(Guid id, string httpRequestType);
    public Task  AuditCover(Guid id, string httpRequestType);
}