using Exercicio5.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Questao5.Application.Movimentacao.Commands;
using Questao5.Application.Movimentacao.Queries;
using Questao5.Application.Saldo.Queries;
using Questao5.Domain.Exceptions;
using Questao5.Services.DTO;

namespace Questao5.Services.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContaCorrenteController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMovimentacaoQueryService _movimentacaoQueryService;

        public ContaCorrenteController(IMediator mediator, IMovimentacaoQueryService movimentacaoQueryService)
        {
            _mediator = mediator;
            _movimentacaoQueryService = movimentacaoQueryService;
        }

        /// <summary>
        /// Realiza uma movimentação em uma conta corrente
        /// </summary>
        /// <param name="request">Dados da movimentação</param>
        /// <returns>ID do movimento criado</returns>
        /// <response code="200">Movimentação realizada com sucesso</response>
        /// <response code="400">Dados inválidos</response>
        [HttpPost("movimentacao")]
        [ProducesResponseType(typeof(GetMovimentacaoByIdQuery), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> Movimentacao([FromBody] MovimentacaoRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .FirstOrDefault();

                    return BadRequest(new ErrorResponse
                    {
                        Tipo = "VALIDATION_ERROR",
                        Mensagem = errors ?? "Dados inválidos"
                    });
                }

                var command = new CreateMovimentacaoCommand
                {
                    IdContaCorrente = request.IdContaCorrente,
                    Valor = request.Valor,
                    TipoMovimento = request.TipoMovimento
                };

                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (BusinessException ex)
            {
                return BadRequest(new ErrorResponse
                {
                    Tipo = ex.Tipo,
                    Mensagem = ex.Message
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorResponse
                {
                    Tipo = "INTERNAL_ERROR",
                    Mensagem = "Erro interno do servidor"
                });
            }
        }

        /// <summary>
        /// Consulta o saldo de uma conta corrente
        /// </summary>
        /// <param name="idContaCorrente">ID da conta corrente</param>
        /// <returns>Dados do saldo da conta</returns>
        /// <response code="200">Saldo consultado com sucesso</response>
        /// <response code="400">Dados inválidos</response>
        [HttpGet("saldo/{idContaCorrente}")]
        [ProducesResponseType(typeof(SaldoResponse), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> Saldo(string idContaCorrente)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(idContaCorrente))
                {
                    return BadRequest(new ErrorResponse
                    {
                        Tipo = "VALIDATION_ERROR",
                        Mensagem = "ID da conta corrente é obrigatório"
                    });
                }

                var result = await _movimentacaoQueryService.GetSaldoAsync(idContaCorrente);
                return Ok(result);
            }
            catch (BusinessException ex)
            {
                return BadRequest(new ErrorResponse
                {
                    Tipo = ex.Tipo,
                    Mensagem = ex.Message
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorResponse
                {
                    Tipo = "INTERNAL_ERROR",
                    Mensagem = "Erro interno do servidor"
                });
            }
        }
    }
}
