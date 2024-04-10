using System.Text.Json.Serialization;
using Claims.Persistence.Cosmos;

namespace Claims
{
    public class Claim : CosmosItem
    {
        public Guid CoverId { get; set; }

        public DateTimeOffset Created { get; set; }

        public string Name { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ClaimType Type { get; set; }

        public decimal DamageCost { get; set; }
    }

    public enum ClaimType
    {
        Collision = 0,
        Grounding = 1,
        BadWeather = 2,
        Fire = 3
    }
}