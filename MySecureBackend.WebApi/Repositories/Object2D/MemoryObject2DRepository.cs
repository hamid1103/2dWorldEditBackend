namespace MySecureBackend.WebApi.Repositories.Object2D
{
    using MySecureBackend.WebApi.Models;
    public class MemoryObject2DRepository : IObjectRepository
    {

        private static List<Object2D> _object2Ds = [];

        public Task<IEnumerable<Object2D>> SelectAsync(Environment2D environment2D)
        {
            return Task.FromResult(_object2Ds.FindAll(o => o.EnvironmentId == environment2D.Id).AsEnumerable());
        }

        
        
        public Task InsertAsync(Object2D object2D)
        {
            _object2Ds.Add(object2D);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Guid id)
        {
            Object2D? object2D = _object2Ds.Find(o => o.Id == id);
            if(object2D!=null)
                _object2Ds.Remove(object2D);
            return Task.CompletedTask;
        }

        public Task DeleteByEnvironment(Environment2D environment2D)
        {
            foreach (var object2D in _object2Ds)
            {
                if (object2D.EnvironmentId == environment2D.Id)
                {
                    _object2Ds.Remove(object2D);
                }
            }

            return Task.CompletedTask;
        }

        public Task<IEnumerable<Object2D>> SelectAsync()
        {
            return Task.FromResult(_object2Ds.AsEnumerable());
        }

        public Task<bool> ObjectExistsAsync(Guid environmentId, int posX, int posY)
        {
            return Task.FromResult(_object2Ds.Exists(x => x.EnvironmentId == environmentId && x.PosX == posX && x.PosY == posY));
        }

        public Task<Object2D?> SelectAsync(Guid id)
        {
            return Task.FromResult(_object2Ds.SingleOrDefault(x => x.Id == id));
        }

        public async Task UpdateAsync(Object2D object2D)
        {
            await DeleteAsync((Guid)object2D.Id);
            await InsertAsync(object2D);
        }
    }
}