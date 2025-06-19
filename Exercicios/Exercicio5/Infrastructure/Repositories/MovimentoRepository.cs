using Dapper;
using Questao5.Domain.Entities;
using Questao5.Domain.Interfaces;
using System.Data;

namespace Questao5.Infrastructure.Repositories
{
    public class MovimentoRepository : IMovimentoRepository
    {
        private readonly IDbConnection _connection;

        public MovimentoRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<string> CreateAsync(Movimento movimento)
        {
            var sql = @"INSERT INTO movimento (idmovimento, idcontacorrente, datamovimento, tipomovimento, valor) 
                       VALUES (@IdMovimento, @IdContaCorrente, @DataMovimento, @TipoMovimento, @Valor)";

            await _connection.ExecuteAsync(sql, movimento);
            return movimento.IdMovimento;
        }

        public async Task<decimal> GetSaldoAsync(string idContaCorrente)
        {
            var sql = @"
                SELECT 
                    COALESCE(SUM(CASE WHEN tipomovimento = 'C' THEN valor ELSE 0 END), 0) - 
                    COALESCE(SUM(CASE WHEN tipomovimento = 'D' THEN valor ELSE 0 END), 0) as Saldo
                FROM movimento 
                WHERE idcontacorrente = @IdContaCorrente";

            return await _connection.QueryFirstOrDefaultAsync<decimal>(sql, new { IdContaCorrente = idContaCorrente });
        }
    }
}
