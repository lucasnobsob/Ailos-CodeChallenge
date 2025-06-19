using MediatR;

namespace Questao5.Application.Saldo.Queries
{
    public class GetSaldoByContaCorrenteQuery : IRequest<SaldoResponse>
    {
        public string IdContaCorrente { get; set; }
    }
}
