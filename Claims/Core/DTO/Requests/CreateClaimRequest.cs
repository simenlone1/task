using Claims.Util;

namespace Claims.Core.DTO.Requests;

public class CreateClaimRequest
{
        public string CoverId { get; set; }
        public DateTimeOffset Created { get; set; }
        public string Name { get; set; }
        public ClaimType Type { get; set; }
       [ClaimDamageRange] 
        public decimal DamageCost { get; set; }
        
}