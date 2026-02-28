using MySecureBackend.WebApi.Models;

namespace MySecureBackend.WebApi.Repositories
{
    public interface IExampleObjectRepository
    {
        Task InsertAsync(ExampleObject exampleObject);
        Task DeleteAsync(Guid id);
        Task<IEnumerable<ExampleObject>> SelectAsync();
        Task<ExampleObject?> SelectAsync(Guid id);
        Task UpdateAsync(ExampleObject exampleObject);
    }
}