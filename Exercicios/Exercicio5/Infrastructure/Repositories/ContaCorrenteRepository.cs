using Dapper;
using Questao5.Domain.Entities;
using Questao5.Domain.Interfaces;
using System.Data;

namespace Questao5.Infrastructure.Repositories
{
    public class ContaCorrenteRepository : IContaCorrenteRepository
    {
        private readonly IDbConnection _connection;

        public ContaCorrenteRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<ContaCorrente?> GetByIdAsync(string idContaCorrente)
        {
            var sql = "SELECT idcontacorrente as IdContaCorrente, numero as Numero, nome as Nome, ativo as Ativo FROM contacorrente WHERE idcontacorrente = @IdContaCorrente";
            return await _connection.QueryFirstOrDefaultAsync<ContaCorrente>(sql, new { IdContaCorrente = idContaCorrente });
        }
    }
}
