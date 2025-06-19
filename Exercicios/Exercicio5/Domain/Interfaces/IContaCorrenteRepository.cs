﻿using Questao5.Domain.Entities;

namespace Questao5.Domain.Interfaces
{
    public interface IContaCorrenteRepository
    {
        Task<ContaCorrente?> GetByIdAsync(string idContaCorrente);
    }
}
