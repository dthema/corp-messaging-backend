using System.Data;
using Dapper;
using Domain.Models;
using Domain.Repositories;
using Npgsql;

namespace Infrastructure.Repositories;

public class SessionRepository : BaseRepository, ISessionRepository
{
    public SessionRepository()
    {
        SimpleCRUD.SetDialect(SimpleCRUD.Dialect.PostgreSQL);
    }
    
    public async Task<int?> InsertSessionAsync(Session session, IDbTransaction? transaction = null) =>
        await InsertAsync(session, transaction);

    public async Task<int> DeleteSessionAsync(int id, IDbTransaction? transaction = null) => 
        await DeleteAsync<Session>(id, transaction);

    public async Task DeleteAllSessionsAsync(IDbTransaction? transaction = null) =>
        await DeleteAllAsync<Session>(transaction);

    public async Task<Session> GetSessionAsync(int id, IDbTransaction? transaction = null) =>
        await GetAsync<Session>(id, transaction);

    public async Task<IEnumerable<Session>> GetAllSessionsAsync(IDbTransaction? transaction = null) =>
        await GetListAsync<Session>(transaction);

    public async Task<Session?> GetSessionByWorkerIdAsync(int workerId, IDbTransaction? transaction = null)
    {
        await using var connection = new NpgsqlConnection(ConnectionString);
        var accounts = await connection.GetListAsync<Session>(
            "WHERE \"WorkerId\" = @Id",
            new { Id = workerId }, transaction);
        return accounts.FirstOrDefault(x => x.WorkerId == workerId);
    }
}