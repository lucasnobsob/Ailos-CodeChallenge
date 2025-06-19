using NSubstitute;
using Questao5.Application.Movimentacao.Commands;
using Questao5.Application.Movimentacao.Handlers;
using Questao5.Application.Movimentacao.Queries;
using Questao5.Domain.Entities;
using Questao5.Domain.Exceptions;
using Questao5.Domain.Interfaces;
using System.Text.Json;
using Tests.Exercicio5.Dummies;

namespace Tests.Exercicio5.Application
{
    public class CreateMovimentacaoCommandHandlerTests
    {
        private readonly IContaCorrenteRepository _contaCorrenteRepository;
        private readonly IMovimentoRepository _movimentoRepository;
        private readonly IIdempotenciaRepository _idempotenciaRepository;
        private readonly MovimentacaoCommandHandler _handler;

        public CreateMovimentacaoCommandHandlerTests()
        {
            _contaCorrenteRepository = Substitute.For<IContaCorrenteRepository>();
            _movimentoRepository = Substitute.For<IMovimentoRepository>();
            _idempotenciaRepository = Substitute.For<IIdempotenciaRepository>();
            _handler = new MovimentacaoCommandHandler(
                _contaCorrenteRepository,
                _movimentoRepository,
                _idempotenciaRepository);
        }

        [Fact]
        public async Task Handle_ComChaveIdempotenciaExistente_DeveRetornarResultadoExistente()
        {
            // Arrange
            var command = new CreateMovimentacaoCommand
            {
                ChaveIdempotencia = "test-key",
                IdContaCorrente = "conta-123",
                Valor = 100,
                TipoMovimento = "C"
            };

            var idempotenciaExistente = new Idempotencia
            {
                ChaveIdempotencia = "test-key",
                Resultado = JsonSerializer.Serialize(new GetMovimentacaoByIdQuery { Id = "movimento-123" })
            };

            _idempotenciaRepository.GetByChaveAsync("test-key")
                .Returns(idempotenciaExistente);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal("movimento-123", result.Id);
            await _contaCorrenteRepository.DidNotReceive().GetByIdAsync(Arg.Any<string>());
            await _movimentoRepository.DidNotReceive().CreateAsync(Arg.Any<Movimento>());
        }

        [Fact]
        public async Task Handle_ComContaInexistente_DeveLancarBusinessException()
        {
            // Arrange
            var command = new CreateMovimentacaoCommand
            {
                ChaveIdempotencia = Guid.NewGuid().ToString(),
                IdContaCorrente = "conta-inexistente",
                Valor = 100,
                TipoMovimento = "C"
            };

            _idempotenciaRepository.GetByChaveAsync(command.ChaveIdempotencia)
                .Returns((Idempotencia?)null);

            _contaCorrenteRepository.GetByIdAsync(command.IdContaCorrente)
                .Returns((ContaCorrente?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<BusinessException>(
                () => _handler.Handle(command, CancellationToken.None));

            Assert.Equal("INVALID_ACCOUNT", exception.Tipo);
            Assert.Equal("Conta corrente não encontrada", exception.Message);
        }

        [Fact]
        public async Task Handle_ComContaInativa_DeveLancarBusinessException()
        {
            // Arrange
            var contaInativa = TestDataBuilder.CreateContaInativa();
            var command = new CreateMovimentacaoCommand
            {
                ChaveIdempotencia = Guid.NewGuid().ToString(),
                IdContaCorrente = contaInativa.IdContaCorrente,
                Valor = 100,
                TipoMovimento = "C"
            };

            _idempotenciaRepository.GetByChaveAsync(command.ChaveIdempotencia)
                .Returns((Idempotencia?)null);

            _contaCorrenteRepository.GetByIdAsync(command.IdContaCorrente)
                .Returns(contaInativa);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<BusinessException>(
                () => _handler.Handle(command, CancellationToken.None));

            Assert.Equal("INACTIVE_ACCOUNT", exception.Tipo);
            Assert.Equal("Conta corrente inativa", exception.Message);
        }

        [Fact]
        public async Task Handle_ComValorNegativo_DeveLancarBusinessException()
        {
            // Arrange
            var contaAtiva = TestDataBuilder.CreateContaAtiva();
            var command = new CreateMovimentacaoCommand
            {
                ChaveIdempotencia = Guid.NewGuid().ToString(),
                IdContaCorrente = contaAtiva.IdContaCorrente,
                Valor = -100,
                TipoMovimento = "C"
            };

            _idempotenciaRepository.GetByChaveAsync(command.ChaveIdempotencia)
                .Returns((Idempotencia?)null);

            _contaCorrenteRepository.GetByIdAsync(command.IdContaCorrente)
                .Returns(contaAtiva);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<BusinessException>(
                () => _handler.Handle(command, CancellationToken.None));

            Assert.Equal("INVALID_VALUE", exception.Tipo);
            Assert.Equal("Valor deve ser positivo", exception.Message);
        }

        [Fact]
        public async Task Handle_ComTipoMovimentoInvalido_DeveLancarBusinessException()
        {
            // Arrange
            var contaAtiva = TestDataBuilder.CreateContaAtiva();
            var command = new CreateMovimentacaoCommand
            {
                ChaveIdempotencia = Guid.NewGuid().ToString(),
                IdContaCorrente = contaAtiva.IdContaCorrente,
                Valor = 100,
                TipoMovimento = "X"
            };

            _idempotenciaRepository.GetByChaveAsync(command.ChaveIdempotencia)
                .Returns((Idempotencia?)null);

            _contaCorrenteRepository.GetByIdAsync(command.IdContaCorrente)
                .Returns(contaAtiva);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<BusinessException>(
                () => _handler.Handle(command, CancellationToken.None));

            Assert.Equal("INVALID_TYPE", exception.Tipo);
            Assert.Equal("Tipo de movimento deve ser 'C' (Crédito) ou 'D' (Débito)", exception.Message);
        }

        [Fact]
        public async Task Handle_ComDadosValidos_DeveCriarMovimentoESalvarIdempotencia()
        {
            // Arrange
            var contaAtiva = TestDataBuilder.CreateContaAtiva();
            var command = new CreateMovimentacaoCommand
            {
                ChaveIdempotencia = Guid.NewGuid().ToString(),
                IdContaCorrente = contaAtiva.IdContaCorrente,
                Valor = 100,
                TipoMovimento = "C"
            };

            var idMovimentoEsperado = Guid.NewGuid().ToString();

            _idempotenciaRepository.GetByChaveAsync(command.ChaveIdempotencia)
                .Returns((Idempotencia?)null);

            _contaCorrenteRepository.GetByIdAsync(command.IdContaCorrente)
                .Returns(contaAtiva);

            _movimentoRepository.CreateAsync(Arg.Any<Movimento>())
                .Returns(idMovimentoEsperado);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(idMovimentoEsperado, result.Id);

            await _movimentoRepository.Received(1).CreateAsync(Arg.Is<Movimento>(m =>
                m.IdContaCorrente == command.IdContaCorrente &&
                m.TipoMovimento == command.TipoMovimento &&
                m.Valor == command.Valor));

            await _idempotenciaRepository.Received(1).CreateAsync(Arg.Is<Idempotencia>(i =>
                i.ChaveIdempotencia == command.ChaveIdempotencia));
        }

        [Theory]
        [InlineData("C")]
        [InlineData("D")]
        public async Task Handle_ComTiposMovimentoValidos_DeveProcessarCorretamente(string tipoMovimento)
        {
            // Arrange
            var contaAtiva = TestDataBuilder.CreateContaAtiva();
            var command = new CreateMovimentacaoCommand
            {
                ChaveIdempotencia = Guid.NewGuid().ToString(),
                IdContaCorrente = contaAtiva.IdContaCorrente,
                Valor = 100,
                TipoMovimento = tipoMovimento
            };

            _idempotenciaRepository.GetByChaveAsync(command.ChaveIdempotencia)
                .Returns((Idempotencia?)null);

            _contaCorrenteRepository.GetByIdAsync(command.IdContaCorrente)
                .Returns(contaAtiva);

            _movimentoRepository.CreateAsync(Arg.Any<Movimento>())
                .Returns(Guid.NewGuid().ToString());

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result.Id);
        }
    }
}
