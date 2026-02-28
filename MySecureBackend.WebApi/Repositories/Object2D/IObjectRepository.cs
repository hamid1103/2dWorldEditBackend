namespace MySecureBackend.WebApi.Repositories.Object2D
{
    public interface IObjectRepository
    {
        Task<IEnumerable<Models.Object2D>> SelectAsync(Models.Environment2D environment2D);
        Task InsertAsync(Models.Object2D object2D);
        Task DeleteAsync(Guid id);
        Task DeleteByEnvironment(Models.Environment2D environment2D);
        Task<IEnumerable<Models.Object2D>> SelectAsync();
        Task<bool> ObjectExistsAsync(Guid environmentId, int posX, int posY);
        Task<Models.Object2D?> SelectAsync(Guid id);
        Task UpdateAsync(Models.Object2D object2D);
    }
}

