namespace MySecureBackend.WebApi.Repositories.Environment2D
{
    using MySecureBackend.WebApi.Models;
    public class MemoryEnvironment2DRepository : IEnvironmentRepository
    {
        private static List<Environment2D> environment2Ds = [];
        
        public Task InsertAsync(Environment2D environment2dObject)
        {
            environment2Ds.Add(environment2dObject);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Guid id)
        {
            Environment2D? environment2D = environment2Ds.Find(e => e.Id == id);
            if(environment2D!=null)
                environment2Ds.Remove(environment2D);
            return Task.CompletedTask;
        }

        public Task<IEnumerable<Environment2D>> SelectAsync()
        {
            return Task.FromResult(environment2Ds.AsEnumerable());
        }

        public Task<IEnumerable<Environment2D>> SelectAsync(string userId)
        {
            List<Environment2D> selectedENVS = environment2Ds.FindAll((x) => x.UserId == userId);
            return Task.FromResult(selectedENVS.AsEnumerable());
        }

        public Task<Environment2D?> SelectAsync(Guid id)
        {
            return Task.FromResult(environment2Ds.SingleOrDefault(x => x.Id == id));
        }

        public async Task UpdateAsync(Environment2D environment2dObject)
        {
            await DeleteAsync((Guid)environment2dObject.Id);
            await InsertAsync(environment2dObject);
        }
    }   
}