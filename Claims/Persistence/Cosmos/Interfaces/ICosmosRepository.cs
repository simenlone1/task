using Claims.Persistence;
using Claims.Persistence.Cosmos;

namespace Claims.Presistence.Cosmos;

public interface ICosmosRepository<T> where T: CosmosItem
{
    Task<IEnumerable<T>> GetItemsAsync();
    Task<T?> GetItemAsync(Guid id);
    Task AddItemAsync(T item);
    Task<bool> DeleteItemAsync(Guid id);
}