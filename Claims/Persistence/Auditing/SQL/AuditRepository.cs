using Claims.Auditing;
using Claims.Persistence.ServiceBus.Interface;

namespace Claims.Persistence.Auditing.SQL
{
    public class AuditRepository(AuditContext auditContext) : IAuditRepository
    {
        public async Task AuditClaim(Guid id, string httpRequestType)
        {
            var claimAudit = new ClaimAudit()
            {
                Created = DateTimeOffset.Now,
                HttpRequestType = httpRequestType,
                ClaimId = id.ToString()
            };

            await auditContext.AddAsync(claimAudit);
            await auditContext.SaveChangesAsync();
        }
        
        public async Task AuditCover(Guid id, string httpRequestType)
        {
            var coverAudit = new CoverAudit()
            {
                Created = DateTimeOffset.Now,
                HttpRequestType = httpRequestType,
                CoverId = id.ToString()
            };

            await auditContext.AddAsync(coverAudit);
            await auditContext.SaveChangesAsync();
        }
    }
}
