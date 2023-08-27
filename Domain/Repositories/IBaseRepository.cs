using System.Data;
using Npgsql;

namespace Domain.Repositories;

public interface IBaseRepository
{
    Task<IDbTransaction> BeginTransactionAsync();
}