using Microsoft.Data.Sqlite;
using Questao5.Domain.Interfaces;
using System.Data;

namespace Questao5.Infrastructure.Sqlite
{
    public class DatabaseConnection : IDatabaseConnection
    {
        private readonly string _connectionString;

        public DatabaseConnection(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IDbConnection CreateConnection()
        {
            return new SqliteConnection(_connectionString);
        }
    }
}
