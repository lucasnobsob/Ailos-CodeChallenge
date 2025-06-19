using Dapper;
using Questao5.Domain.Entities;
using Questao5.Domain.Interfaces;
using System.Data;

namespace Questao5.Infrastructure.Repositories
{
    public class IdempotenciaRepository : IIdempotenciaRepository
    {
        private readonly IDbConnection _connection;

        public IdempotenciaRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<Idempotencia?> GetByChaveAsync(string chaveIdempotencia)
        {
            var sql = "SELECT chave_idempotencia as ChaveIdempotencia, requisicao as Requisicao, resultado as Resultado FROM idempotencia WHERE chave_idempotencia = @ChaveIdempotencia";
            return await _connection.QueryFirstOrDefaultAsync<Idempotencia>(sql, new { ChaveIdempotencia = chaveIdempotencia });
        }

        public async Task CreateAsync(Idempotencia idempotencia)
        {
            var sql = "INSERT INTO idempotencia (chave_idempotencia, requisicao, resultado) VALUES (@ChaveIdempotencia, @Requisicao, @Resultado)";
            await _connection.ExecuteAsync(sql, idempotencia);
        }
    }
}
