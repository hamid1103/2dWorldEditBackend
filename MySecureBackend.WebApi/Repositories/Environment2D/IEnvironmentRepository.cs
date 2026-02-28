namespace MySecureBackend.WebApi.Repositories.Environment2D
{
    public interface IEnvironmentRepository
    {
        Task InsertAsync(Models.Environment2D environment2dObject);
        Task DeleteAsync(Guid id);
        Task<IEnumerable<Models.Environment2D>> SelectAsync();
        Task<IEnumerable<Models.Environment2D>> SelectAsync(string userId);
        Task<Models.Environment2D?> SelectAsync(Guid id);
        Task UpdateAsync(Models.Environment2D environment2dObject);
    }
}

