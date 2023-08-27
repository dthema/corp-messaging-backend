using System.Data;
using Domain.Models;

namespace Domain.Repositories;

public interface ISessionRepository : IBaseRepository
{
    Task<int?> InsertSessionAsync(Session session, IDbTransaction? transaction = null);
    Task<int> DeleteSessionAsync(int id, IDbTransaction? transaction = null);
    Task DeleteAllSessionsAsync(IDbTransaction? transaction = null);
    Task<Session> GetSessionAsync(int id, IDbTransaction? transaction = null); 
    Task<IEnumerable<Session>> GetAllSessionsAsync(IDbTransaction? transaction = null);
    Task<Session?> GetSessionByWorkerIdAsync(int workerId, IDbTransaction? transaction = null);
}