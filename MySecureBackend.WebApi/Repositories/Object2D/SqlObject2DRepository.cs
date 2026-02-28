using Dapper;
using Microsoft.Data.SqlClient;

namespace MySecureBackend.WebApi.Repositories.Object2D;
using Models;
public class SqlObject2DRepository : IObjectRepository
{
    private readonly string sqlConnectionString;

    public SqlObject2DRepository(string sqlConnectionString)
    {
        this.sqlConnectionString = sqlConnectionString;
    }

    public async Task<IEnumerable<Object2D>> SelectAsync(Environment2D environment2D)
    {
        using (var sqlConnection = new SqlConnection(sqlConnectionString))
        {
            return await sqlConnection.QueryAsync<Object2D>("SELECT * FROM [Object2D] WHERE EnvironmentId = @Id", new { environment2D.Id });   
        }
    }

    public async Task<bool> ObjectExistsAsync(Guid environmentId, int posX, int posY)
    {
        using (var sqlConnection = new SqlConnection(sqlConnectionString))
        {
            return ((int)sqlConnection.ExecuteScalar("SELECT count(*) as IsExists FROM [Object2D] WHERE EnvironmentId = @environmentId AND PosX = @PosX AND PosY = @PosY", new {environmentId, PosX = posX, PosY = posY}) == 1);
        }
    }
    
    public async Task InsertAsync(Object2D object2D)
    {
        using (var sqlConnection = new SqlConnection(sqlConnectionString))
        {
            await sqlConnection.ExecuteAsync("INSERT INTO [Object2D] (Id, PosX, PosY, TileId, Layer, EnvironmentId) VALUES (@Id, @PosX, @PosY, @TileId, @Layer, @EnvironmentId)", object2D);
        }
    }

    public async Task<Object2D?> SelectAsync(Guid id)
    {
        using (var sqlConnection = new SqlConnection(sqlConnectionString))
        {
            return await sqlConnection.QuerySingleOrDefaultAsync<Object2D>("SELECT * FROM [Object2D] WHERE Id = @Id", new { id });   
        }
    }

    public async Task DeleteByEnvironment(Environment2D environment2D)
    {
        using (var sqlConnection = new SqlConnection(sqlConnectionString))
        {
            string id = environment2D.Id.ToString();
            await sqlConnection.ExecuteAsync("DELETE FROM [Object2D] WHERE EnvironmentId = @Id", new { id });   
        }
    }

    public async Task<IEnumerable<Object2D>> SelectAsync()
    {
        using (var sqlConnection = new SqlConnection(sqlConnectionString))
        {
            return await sqlConnection.QueryAsync<Object2D>("SELECT * FROM [Object2D]");
        }
    }

    //Wanneer een Object wordt aangepast, verandert alleen TileId. Die representeert wat voor Tile het is.
    //Positie verandert niet
    public async Task UpdateAsync(Object2D exampleObject)
    {
        using (var sqlConnection = new SqlConnection(sqlConnectionString))
        {
            await sqlConnection.ExecuteAsync("UPDATE [Object2D] SET " +
                                             "TileId = @TileId " +
                                             "WHERE Id = @Id", exampleObject);
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        using (var sqlConnection = new SqlConnection(sqlConnectionString))
        {
            await sqlConnection.ExecuteAsync("DELETE FROM [Object2D] WHERE Id = @Id", new { id });
        }
    }
}