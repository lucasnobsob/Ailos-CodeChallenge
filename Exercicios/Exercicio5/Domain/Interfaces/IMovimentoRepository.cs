using Questao5.Domain.Entities;

namespace Questao5.Domain.Interfaces
{
    public interface IMovimentoRepository
    {
        Task<string> CreateAsync(Movimento movimento);
        Task<decimal> GetSaldoAsync(string idContaCorrente);
    }
}
