using MediatR;
using Questao5.Application.Movimentacao.Queries;

namespace Questao5.Application.Movimentacao.Commands
{
    public class CreateMovimentacaoCommand : IRequest<GetMovimentacaoByIdQuery>
    {
        public string ChaveIdempotencia { get; set; }
        public string IdContaCorrente { get; set; }
        public decimal Valor { get; set; }
        public string TipoMovimento { get; set; }
    }
}
