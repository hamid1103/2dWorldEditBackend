using Dapper;
using Microsoft.Data.SqlClient;

namespace MySecureBackend.WebApi.Repositories.Environment2D;
using Models;

public class SqlEnvironment2dRepository : IEnvironmentRepository
{
    private readonly string sqlConnectionString;

    public SqlEnvironment2dRepository(string sqlConnectionString)
    {
        this.sqlConnectionString = sqlConnectionString;
    }
    
    public async Task InsertAsync(Environment2D environment2dObject)
    {
        using (var sqlConnection = new SqlConnection(sqlConnectionString))
        {
            await sqlConnection.ExecuteAsync("INSERT INTO [Environment2D] (Id, Name, PreviewURL, UserId) VALUES (@Id, @Name, @PreviewURL, @UserId)", environment2dObject);
        }
    }

    public async Task<Environment2D?> SelectAsync(Guid id)
    {
        using (var sqlConnection = new SqlConnection(sqlConnectionString))
        {
            return await sqlConnection.QuerySingleOrDefaultAsync<Environment2D>("SELECT * FROM [Environment2D] WHERE Id = @Id", new { id });   
        }
    }
    
    public async Task<IEnumerable<Environment2D>> SelectAsync()
    {
        using (var sqlConnection = new SqlConnection(sqlConnectionString))
        {
            return await sqlConnection.QueryAsync<Environment2D>("SELECT * FROM [Environment2D]");   
        }
    }

    public async Task<IEnumerable<Environment2D>> SelectAsync(string userId)
    {
        using (var sqlConnection = new SqlConnection(sqlConnectionString))
        {
            return await sqlConnection.QueryAsync<Environment2D>("SELECT * FROM [Environment2D] WHERE UserId = @userId", new {userId});   
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        using (var sqlConnection = new SqlConnection(sqlConnectionString))
        {
            await sqlConnection.ExecuteAsync("DELETE FROM [Environment2D] WHERE Id = @Id", new { id });
        }
    }

    

    public async Task UpdateAsync(Environment2D environment2dObject)
    {
        using (var sqlConnection = new SqlConnection(sqlConnectionString))
        {
            await sqlConnection.ExecuteAsync("UPDATE [Environment2D] SET " +
                                             "Name = @Name, " +
                                             "PreviewURL = @PreviewURL " +
                                             "WHERE Id = @Id", environment2dObject);
        }
    }
}