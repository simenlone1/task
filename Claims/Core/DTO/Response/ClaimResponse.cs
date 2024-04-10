using System.Text.Json.Serialization;

namespace Claims.Core.DTO.Response;

public class ClaimResponse
{
        public string Id { get; set; }
        
        public string CoverId { get; set; }

        public DateTimeOffset Created { get; set; }

        public string Name { get; set; }

        public ClaimType Type { get; set; }

        public decimal DamageCost { get; set; }
}