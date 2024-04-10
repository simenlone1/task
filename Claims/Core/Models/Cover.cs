using System.Text.Json.Serialization;
using Claims.Persistence;
using Claims.Persistence.Cosmos;

namespace Claims;

public class Cover: CosmosItem
{
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public CoverType Type { get; set; }
    public decimal Premium { get; set; }
}

public enum CoverType
{
    Yacht = 0,
    PassengerShip = 1,
    ContainerShip = 2,
    BulkCarrier = 3,
    Tanker = 4
}