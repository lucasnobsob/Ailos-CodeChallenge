
namespace Exercicio5.Domain.Interfaces
{
    public interface IMovimentacaoQueryService
    {
        Task<decimal> GetSaldoAsync(string idContaCorrente);
    }
}
