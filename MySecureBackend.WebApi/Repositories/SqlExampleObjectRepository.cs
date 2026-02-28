using Dapper;
using Microsoft.Data.SqlClient;
using MySecureBackend.WebApi.Models;

namespace MySecureBackend.WebApi.Repositories
{
    public class SqlExampleObjectRepository : IExampleObjectRepository
    {
        private readonly string sqlConnectionString;

        public SqlExampleObjectRepository(string sqlConnectionString)
        {
            this.sqlConnectionString = sqlConnectionString;
        }

        public async Task InsertAsync(ExampleObject exampleObject)
        {
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
               await sqlConnection.ExecuteAsync("INSERT INTO [ExampleObject] (Id, Name, Number) VALUES (@Id, @Name, @Number)", exampleObject);
            }
        }

        public async Task<ExampleObject?> SelectAsync(Guid id)
        {
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                return await sqlConnection.QuerySingleOrDefaultAsync<ExampleObject>("SELECT * FROM [ExampleObject] WHERE Id = @Id", new { id });   
            }
        }

        public async Task<IEnumerable<ExampleObject>> SelectAsync()
        {
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                return await sqlConnection.QueryAsync<ExampleObject>("SELECT * FROM [ExampleObject]");
            }
        }

        public async Task UpdateAsync(ExampleObject exampleObject)
        {
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                await sqlConnection.ExecuteAsync("UPDATE [ExampleObject] SET " +
                                                 "Name = @Name, " +
                                                 "Number = @Number " +
                                                 "WHERE Id = @Id", exampleObject);

            }
        }

        public async Task DeleteAsync(Guid id)
        {
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                await sqlConnection.ExecuteAsync("DELETE FROM [ExampleObject] WHERE Id = @Id", new { id });
            }
        }
    }
}
