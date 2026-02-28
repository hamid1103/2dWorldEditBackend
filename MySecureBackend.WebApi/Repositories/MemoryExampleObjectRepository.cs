using MySecureBackend.WebApi.Models;

namespace MySecureBackend.WebApi.Repositories
{
    public class MemoryExampleObjectRepository : IExampleObjectRepository
    {
        // Ignoring thread safety for simplicity
        private static List<ExampleObject> exampleObjects = [];

        public Task DeleteAsync(Guid id)
        {
            exampleObjects.Remove(exampleObjects.Single(x => x.Id == id));
            return Task.CompletedTask;
        }

        public Task InsertAsync(ExampleObject exampleObject)
        {
            exampleObjects.Add(exampleObject);
            return Task.CompletedTask;
        }

        public Task<IEnumerable<ExampleObject>> SelectAsync()
        {
            return Task.FromResult(exampleObjects.AsEnumerable());
        }

        public Task<ExampleObject?> SelectAsync(Guid id)
        {
            return Task.FromResult(exampleObjects.SingleOrDefault(x => x.Id == id));
        }

        public async Task UpdateAsync(ExampleObject exampleObject)
        {
            await DeleteAsync(exampleObject.Id);
            await InsertAsync(exampleObject);
        }
    }
}
