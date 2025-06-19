using Questao5.Domain.Entities;

namespace Questao5.Domain.Interfaces
{
    public interface IIdempotenciaRepository
    {
        Task CreateAsync(Idempotencia idempotencia);
        Task<Idempotencia?> GetByChaveAsync(string chaveIdempotencia);
    }
}
