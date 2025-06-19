using MediatR;
using Questao5.Application.Movimentacao.Commands;
using Questao5.Application.Movimentacao.Queries;
using Questao5.Domain.Entities;
using Questao5.Domain.Exceptions;
using Questao5.Domain.Interfaces;
using System.Text.Json;

namespace Questao5.Application.Movimentacao.Handlers
{
    public class MovimentacaoCommandHandler : IRequestHandler<CreateMovimentacaoCommand, GetMovimentacaoByIdQuery>
    {
        private readonly IContaCorrenteRepository _contaCorrenteRepository;
        private readonly IMovimentoRepository _movimentoRepository;
        private readonly IIdempotenciaRepository _idempotenciaRepository;

        public MovimentacaoCommandHandler(
            IContaCorrenteRepository contaCorrenteRepository,
            IMovimentoRepository movimentoRepository,
            IIdempotenciaRepository idempotenciaRepository)
        {
            _contaCorrenteRepository = contaCorrenteRepository;
            _movimentoRepository = movimentoRepository;
            _idempotenciaRepository = idempotenciaRepository;
        }

        public async Task<GetMovimentacaoByIdQuery> Handle(CreateMovimentacaoCommand request, CancellationToken cancellationToken)
        {
            // Verificar idempotência
            var idempotenciaExistente = await _idempotenciaRepository.GetByChaveAsync(request.ChaveIdempotencia);
            if (idempotenciaExistente != null)
            {
                return JsonSerializer.Deserialize<GetMovimentacaoByIdQuery>(idempotenciaExistente.Resultado);
            }

            // Validar conta corrente
            var conta = await _contaCorrenteRepository.GetByIdAsync(request.IdContaCorrente);
            if (conta == null)
            {
                throw new BusinessException("INVALID_ACCOUNT", "Conta corrente não encontrada");
            }

            if (conta.Ativo == 0)
            {
                throw new BusinessException("INACTIVE_ACCOUNT", "Conta corrente inativa");
            }

            // Validar valor
            if (request.Valor <= 0)
            {
                throw new BusinessException("INVALID_VALUE", "Valor deve ser positivo");
            }

            // Validar tipo de movimento
            if (request.TipoMovimento != "C" && request.TipoMovimento != "D")
            {
                throw new BusinessException("INVALID_TYPE", "Tipo de movimento deve ser 'C' (Crédito) ou 'D' (Débito)");
            }

            // Criar movimento
            var movimento = new Movimento
            {
                IdMovimento = Guid.NewGuid().ToString(),
                IdContaCorrente = request.IdContaCorrente,
                DataMovimento = DateTime.Now.ToString("dd/MM/yyyy"),
                TipoMovimento = request.TipoMovimento,
                Valor = request.Valor
            };

            var idMovimento = await _movimentoRepository.CreateAsync(movimento);

            var response = new GetMovimentacaoByIdQuery { Id = idMovimento };

            // Salvar idempotência
            var idempotencia = new Idempotencia
            {
                ChaveIdempotencia = request.ChaveIdempotencia,
                Requisicao = JsonSerializer.Serialize(request),
                Resultado = JsonSerializer.Serialize(response)
            };

            await _idempotenciaRepository.CreateAsync(idempotencia);

            return response;
        }
    }
}
