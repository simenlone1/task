using System.Configuration;
using System.Text.Json;
using System.Text.Json.Serialization;
using Claims.Persistence;
using Claims.persistence.Cosmos;
using Claims.Persistence.Cosmos;
using Claims.Persistence.Options;
using Claims.Presistence.Cosmos;
using Cosmos.Samples.Shared;
using Microsoft.Azure.Cosmos;

namespace Claims.Util;

public static class CosmosExtension
{
    public static IServiceCollection AddCosmosRepository<T>(this IServiceCollection services, IConfiguration cosmosOptions) where T: CosmosItem
    {
         var options = cosmosOptions.Get<CosmosOptions>();
         if (options == null) throw new ConfigurationErrorsException("Cosmos configuration error");
         
         JsonSerializerOptions jsonOptions = new()
         {
             DefaultIgnoreCondition = JsonIgnoreCondition.Never,
             WriteIndented = true,
             PropertyNameCaseInsensitive = true,
             PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
         };
         var cosmosClientOptions = new CosmosClientOptions()
         {
             ConnectionMode = ConnectionMode.Direct,
             Serializer = new CosmosSystemTextJsonSerializer(jsonOptions )
         };
         var client = new Microsoft.Azure.Cosmos.CosmosClient(options.Account, options.Key, cosmosClientOptions);
         var database = client.CreateDatabaseIfNotExistsAsync(options.DatabaseName).Result;
         var _ = database.Database.CreateContainerIfNotExistsAsync(options.ContainerName, "/id").Result;
         var cosmosDbService = new CosmosRepository<T>(client, options.DatabaseName, options.ContainerName);
         services.AddSingleton<ICosmosRepository<T>>(cosmosDbService);
         return services;
    }
}
