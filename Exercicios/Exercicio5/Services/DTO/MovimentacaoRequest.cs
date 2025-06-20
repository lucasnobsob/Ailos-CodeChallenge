using System.ComponentModel.DataAnnotations;
using Xunit.Sdk;

namespace Questao5.Services.DTO
{
    public class MovimentacaoRequest
    {
        [Required(ErrorMessage = "O ID da conta corrente é obrigatório")]
        public string IdContaCorrente { get; set; }

        [Required(ErrorMessage = "O valor é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O valor deve ser positivo")]
        public decimal Valor { get; set; }

        [Required(ErrorMessage = "O tipo de movimento é obrigatório")]
        [RegularExpression("^[CD]$", ErrorMessage = "O tipo de movimento deve ser 'C' (Crédito) ou 'D' (Débito)")]
        public string TipoMovimento { get; set; }
    }
}
