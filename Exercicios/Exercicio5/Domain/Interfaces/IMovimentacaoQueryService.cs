
using Questao5.Application.Saldo.Queries;

namespace Exercicio5.Domain.Interfaces
{
    public interface IMovimentacaoQueryService
    {
        Task<SaldoResponse> GetSaldoAsync(string idContaCorrente);
    }
}
