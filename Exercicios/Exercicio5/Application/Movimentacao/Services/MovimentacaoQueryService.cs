using Exercicio5.Domain.Interfaces;
using Questao5.Domain.Interfaces;

namespace Exercicio5.Application.Movimentacao.Services
{
    public class MovimentacaoQueryService : IMovimentacaoQueryService
    {
        private readonly IMovimentoRepository _movimentoRepository;

        public MovimentacaoQueryService(IMovimentoRepository movimentoRepository)
        {
            _movimentoRepository = movimentoRepository;
        }

        public async Task<decimal> GetSaldoAsync(string idContaCorrente)
        {
            return await _movimentoRepository.GetSaldoAsync(idContaCorrente);
        }
    }
}
