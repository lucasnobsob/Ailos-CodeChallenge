using Exercicio5.Domain.Interfaces;
using Questao5.Application.Saldo.Queries;
using Questao5.Domain.Exceptions;
using Questao5.Domain.Interfaces;

namespace Exercicio5.Application.Movimentacao.Services
{
    public class MovimentacaoQueryService : IMovimentacaoQueryService
    {
        private readonly IMovimentoRepository _movimentoRepository;
        private readonly IContaCorrenteRepository _contaCorrenteRepository;

        public MovimentacaoQueryService(IMovimentoRepository movimentoRepository, 
            IContaCorrenteRepository contaCorrenteRepository)
        {
            _movimentoRepository = movimentoRepository;
            _contaCorrenteRepository = contaCorrenteRepository;
        }

        public async Task<SaldoResponse> GetSaldoAsync(string idContaCorrente)
        {
            var conta = await _contaCorrenteRepository.GetByIdAsync(idContaCorrente);
            if (conta == null)
            {
                throw new BusinessException("INVALID_ACCOUNT", "Conta corrente não encontrada");
            }

            if (conta.Ativo == 0)
            {
                throw new BusinessException("INACTIVE_ACCOUNT", "Conta corrente inativa");
            }

            var result = await _movimentoRepository.GetSaldoAsync(idContaCorrente);
            var response = new SaldoResponse 
            { 
                NumeroContaCorrente = conta.Numero,
                NomeTitular = conta.Nome,
                DataHoraResposta = DateTime.Now,
                Saldo = result
            };

            return response;
        }
    }
}
