using Bogus;
using Questao5.Domain.Entities;
using Questao5.Services.DTO;

namespace Tests.Exercicio5.Dummies
{
    public static class TestDataBuilder
    {
        public static Faker<ContaCorrente> ContaCorrenteFaker => new Faker<ContaCorrente>()
            .RuleFor(c => c.IdContaCorrente, f => f.Random.Guid().ToString())
            .RuleFor(c => c.Numero, f => f.Random.Int(100, 999))
            .RuleFor(c => c.Nome, f => f.Person.FullName)
            .RuleFor(c => c.Ativo, f => f.Random.Int(0, 1));

        public static Faker<Movimento> MovimentoFaker => new Faker<Movimento>()
            .RuleFor(m => m.IdMovimento, f => f.Random.Guid().ToString())
            .RuleFor(m => m.IdContaCorrente, f => f.Random.Guid().ToString())
            .RuleFor(m => m.DataMovimento, f => f.Date.Recent().ToString("dd/MM/yyyy"))
            .RuleFor(m => m.TipoMovimento, f => f.PickRandom("C", "D"))
            .RuleFor(m => m.Valor, f => f.Random.Decimal(1, 1000));

        public static Faker<MovimentacaoRequest> MovimentacaoRequestFaker => new Faker<MovimentacaoRequest>()
            .RuleFor(r => r.ChaveIdempotencia, f => f.Random.Guid().ToString())
            .RuleFor(r => r.IdContaCorrente, f => f.Random.Guid().ToString())
            .RuleFor(r => r.Valor, f => f.Random.Decimal(1, 1000))
            .RuleFor(r => r.TipoMovimento, f => f.PickRandom("C", "D"));

        public static Faker<Idempotencia> IdempotenciaFaker => new Faker<Idempotencia>()
            .RuleFor(i => i.ChaveIdempotencia, f => f.Random.Guid().ToString())
            .RuleFor(i => i.Requisicao, f => f.Lorem.Sentence())
            .RuleFor(i => i.Resultado, f => f.Lorem.Sentence());

        public static ContaCorrente CreateContaAtiva()
        {
            var conta = ContaCorrenteFaker.Generate();
            conta.Ativo = 1;
            return conta;
        }

        public static ContaCorrente CreateContaInativa()
        {
            var conta = ContaCorrenteFaker.Generate();
            conta.Ativo = 0;
            return conta;
        }

        public static MovimentacaoRequest CreateMovimentacaoCredito()
        {
            var movimentacao = MovimentacaoRequestFaker.Generate();
            movimentacao.TipoMovimento = "C";
            return movimentacao;
        }

        public static MovimentacaoRequest CreateMovimentacaoDebito()
        {
            var movimentacao = MovimentacaoRequestFaker.Generate();
            movimentacao.TipoMovimento = "D";
            return movimentacao;
        }

        public static MovimentacaoRequest CreateMovimentacaoInvalida()
        {
            var movimentacao = MovimentacaoRequestFaker.Generate();
            movimentacao.Valor = -100;
            movimentacao.TipoMovimento = "X";
            return movimentacao;
        }
    }
}
