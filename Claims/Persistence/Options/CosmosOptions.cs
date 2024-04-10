namespace Claims.Persistence.Options;

public sealed class CosmosOptions
{
    public string Account { get; set; }
    public string Key { get; set; }
    public string DatabaseName { get; set; }
    public string ContainerName { get; set; }
}

