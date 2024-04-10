using System.Net;
using Claims.Persistence.Cosmos;
using Claims.Presistence.Cosmos;
using Microsoft.Azure.Cosmos;

namespace Claims.persistence.Cosmos;

public class CosmosRepository<T>: ICosmosRepository<T> where T: CosmosItem
    
{
        private readonly Container _container;

        public CosmosRepository( CosmosClient dbClient,
            string databaseName,
            string containerName)
        {
            if (dbClient == null) throw new ArgumentNullException(nameof(dbClient));
            _container = dbClient.GetContainer(databaseName, containerName);
        }

        public async Task<IEnumerable<T>> GetItemsAsync()
        {
            var query = _container.GetItemQueryIterator<T>(new QueryDefinition("SELECT * FROM c"));
            var results = new List<T>();
            while (query.HasMoreResults)
            {
                var partialResults = await query.ReadNextAsync();
                results.AddRange(partialResults);
            }
            return results;
        }

        public async Task<T?> GetItemAsync(Guid id)
        {
            try
            {
                var response = await _container.ReadItemAsync<T>(id.ToString(), new PartitionKey(id.ToString()));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return default;
            }
        }

        public Task AddItemAsync(T item)
        {
            return _container.CreateItemAsync(item, new PartitionKey(item.Id.ToString()));
        }

        public async Task<bool> DeleteItemAsync(Guid id)
        {
            var response = await _container.DeleteItemAsync<T>(id.ToString(), new PartitionKey(id.ToString()));
            return response.StatusCode == HttpStatusCode.Created;
        }
}