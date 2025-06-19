using MediatR;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Questao5.Application.Movimentacao.Commands;
using Questao5.Application.Movimentacao.Queries;
using Questao5.Application.Saldo.Queries;
using Questao5.Domain.Exceptions;
using Questao5.Services.Controllers;
using Tests.Exercicio5.Dummies;

namespace Tests.Exercicio5.Services
{
    public class ContaCorrenteControllerTests
    {
        private readonly IMediator _mediator;
        private readonly ContaCorrenteController _controller;

        public ContaCorrenteControllerTests()
        {
            _mediator = Substitute.For<IMediator>();
            _controller = new ContaCorrenteController(_mediator);
        }

        [Fact]
        public async Task Movimentacao_ComDadosValidos_DeveRetornarOkComIdMovimento()
        {
            // Arrange
            var request = TestDataBuilder.CreateMovimentacaoCredito();
            var expectedResponse = new GetMovimentacaoByIdQuery { Id = Guid.NewGuid().ToString() };

            _mediator.Send(Arg.Any<CreateMovimentacaoCommand>())
                .Returns(expectedResponse);

            // Act
            var result = await _controller.Movimentacao(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<GetMovimentacaoByIdQuery>(okResult.Value);
            Assert.Equal(expectedResponse.Id, response.Id);
        }

        [Fact]
        public async Task Movimentacao_ComBusinessException_DeveRetornarBadRequestComErro()
        {
            // Arrange
            var request = TestDataBuilder.CreateMovimentacaoCredito();
            var businessException = new BusinessException("INVALID_ACCOUNT", "Conta não encontrada");

            _mediator.Send(Arg.Any<CreateMovimentacaoCommand>())
                .Returns(Task.FromException<GetMovimentacaoByIdQuery>(businessException));

            // Act
            var result = await _controller.Movimentacao(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var errorResponse = Assert.IsType<ErrorResponse>(badRequestResult.Value);
            Assert.Equal("INVALID_ACCOUNT", errorResponse.Tipo);
            Assert.Equal("Conta não encontrada", errorResponse.Mensagem);
        }

        [Fact]
        public async Task Movimentacao_ComExceptionGenerica_DeveRetornarBadRequestComErroInterno()
        {
            // Arrange
            var request = TestDataBuilder.CreateMovimentacaoCredito();

            _mediator.Send(Arg.Any<CreateMovimentacaoCommand>())
                .Returns(Task.FromException<GetMovimentacaoByIdQuery>(new Exception("Erro inesperado")));

            // Act
            var result = await _controller.Movimentacao(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var errorResponse = Assert.IsType<ErrorResponse>(badRequestResult.Value);
            Assert.Equal("INTERNAL_ERROR", errorResponse.Tipo);
            Assert.Equal("Erro interno do servidor", errorResponse.Mensagem);
        }

        [Fact]
        public async Task Saldo_ComIdContaCorrenteValido_DeveRetornarOkComSaldo()
        {
            // Arrange
            var idContaCorrente = Guid.NewGuid().ToString();
            var expectedResponse = new SaldoResponse
            {
                NumeroContaCorrente = 123,
                NomeTitular = "João Silva",
                DataHoraResposta = DateTime.Now,
                Saldo = 1500.75m
            };

            _mediator.Send(Arg.Any<GetSaldoByContaCorrenteQuery>())
                .Returns(expectedResponse);

            // Act
            var result = await _controller.Saldo(idContaCorrente);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<SaldoResponse>(okResult.Value);
            Assert.Equal(expectedResponse.NumeroContaCorrente, response.NumeroContaCorrente);
            Assert.Equal(expectedResponse.NomeTitular, response.NomeTitular);
            Assert.Equal(expectedResponse.Saldo, response.Saldo);
        }

        [Fact]
        public async Task Saldo_ComIdContaCorrenteVazio_DeveRetornarBadRequestComErroValidacao()
        {
            // Arrange
            var idContaCorrente = "";

            // Act
            var result = await _controller.Saldo(idContaCorrente);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var errorResponse = Assert.IsType<ErrorResponse>(badRequestResult.Value);
            Assert.Equal("VALIDATION_ERROR", errorResponse.Tipo);
            Assert.Equal("ID da conta corrente é obrigatório", errorResponse.Mensagem);
        }

        [Fact]
        public async Task Saldo_ComIdContaCorrenteNull_DeveRetornarBadRequestComErroValidacao()
        {
            // Arrange
            string? idContaCorrente = null;

            // Act
            var result = await _controller.Saldo(idContaCorrente!);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var errorResponse = Assert.IsType<ErrorResponse>(badRequestResult.Value);
            Assert.Equal("VALIDATION_ERROR", errorResponse.Tipo);
            Assert.Equal("ID da conta corrente é obrigatório", errorResponse.Mensagem);
        }

        [Fact]
        public async Task Saldo_ComBusinessException_DeveRetornarBadRequestComErro()
        {
            // Arrange
            var idContaCorrente = Guid.NewGuid().ToString();
            var businessException = new BusinessException("INACTIVE_ACCOUNT", "Conta inativa");

            _mediator.Send(Arg.Any<GetSaldoByContaCorrenteQuery>())
                .Returns(Task.FromException<SaldoResponse>(businessException));

            // Act
            var result = await _controller.Saldo(idContaCorrente);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var errorResponse = Assert.IsType<ErrorResponse>(badRequestResult.Value);
            Assert.Equal("INACTIVE_ACCOUNT", errorResponse.Tipo);
            Assert.Equal("Conta inativa", errorResponse.Mensagem);
        }

        [Fact]
        public async Task Saldo_ComExceptionGenerica_DeveRetornarBadRequestComErroInterno()
        {
            // Arrange
            var idContaCorrente = Guid.NewGuid().ToString();

            _mediator.Send(Arg.Any<GetSaldoByContaCorrenteQuery>())
                .Returns(Task.FromException<SaldoResponse>(new Exception("Erro inesperado")));

            // Act
            var result = await _controller.Saldo(idContaCorrente);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var errorResponse = Assert.IsType<ErrorResponse>(badRequestResult.Value);
            Assert.Equal("INTERNAL_ERROR", errorResponse.Tipo);
            Assert.Equal("Erro interno do servidor", errorResponse.Mensagem);
        }

        [Fact]
        public async Task Movimentacao_DeveEnviarComandoCorretoParaMediator()
        {
            // Arrange
            var request = TestDataBuilder.CreateMovimentacaoCredito();
            var expectedResponse = new GetMovimentacaoByIdQuery { Id = Guid.NewGuid().ToString() };

            _mediator.Send(Arg.Any<CreateMovimentacaoCommand>())
                .Returns(expectedResponse);

            // Act
            await _controller.Movimentacao(request);

            // Assert
            await _mediator.Received(1).Send(Arg.Is<CreateMovimentacaoCommand>(cmd =>
                cmd.ChaveIdempotencia == request.ChaveIdempotencia &&
                cmd.IdContaCorrente == request.IdContaCorrente &&
                cmd.Valor == request.Valor &&
                cmd.TipoMovimento == request.TipoMovimento));
        }

        [Fact]
        public async Task Saldo_DeveEnviarQueryCorretaParaMediator()
        {
            // Arrange
            var idContaCorrente = Guid.NewGuid().ToString();
            var expectedResponse = new SaldoResponse
            {
                NumeroContaCorrente = 123,
                NomeTitular = "João Silva",
                DataHoraResposta = DateTime.Now,
                Saldo = 1500.75m
            };

            _mediator.Send(Arg.Any<GetSaldoByContaCorrenteQuery>())
                .Returns(expectedResponse);

            // Act
            await _controller.Saldo(idContaCorrente);

            // Assert
            await _mediator.Received(1).Send(Arg.Is<GetSaldoByContaCorrenteQuery>(query =>
                query.IdContaCorrente == idContaCorrente));
        }
    }
}
