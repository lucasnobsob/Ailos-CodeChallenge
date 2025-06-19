using System.Data;

namespace Questao5.Domain.Interfaces
{
    public interface IDatabaseConnection
    {
        IDbConnection CreateConnection();
    }
}
