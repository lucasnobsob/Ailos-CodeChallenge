namespace Questao5.Application.Saldo.Queries
{
    public class SaldoResponse
    {
        public int NumeroContaCorrente { get; set; }
        public string NomeTitular { get; set; }
        public DateTime DataHoraResposta { get; set; }
        public decimal Saldo { get; set; }
    }
}
