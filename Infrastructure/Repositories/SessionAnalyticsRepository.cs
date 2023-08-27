using System.Data;
using Dapper;
using Domain.Models;
using Domain.Repositories;
using Npgsql;

namespace Infrastructure.Repositories;

public class SessionAnalyticsRepository : BaseRepository, ISessionAnalyticsRepository
{
    public SessionAnalyticsRepository()
    {
        SimpleCRUD.SetDialect(SimpleCRUD.Dialect.PostgreSQL);
    }
    
    public async Task<int?> InsertSessionAnalyticsAsync(SessionAnalytics sessionAnalytics, IDbTransaction? transaction = null) =>
        await InsertAsync(sessionAnalytics);

    public async Task<int> DeleteSessionAnalyticsAsync(int id, IDbTransaction? transaction = null) =>
        await DeleteAsync<SessionAnalytics>(id, transaction);

    public async Task DeleteAllSessionsAnalyticsAsync(IDbTransaction? transaction = null) =>
        await DeleteAllAsync<SessionAnalytics>(transaction);

    public async Task<int> UpdateSessionAnalyticsAsync(SessionAnalytics sessionAnalytics, IDbTransaction? transaction = null) =>
        await UpdateAsync(sessionAnalytics, transaction);

    public async Task<SessionAnalytics> GetSessionAnalyticsAsync(int id, IDbTransaction? transaction = null) =>
        await GetAsync<SessionAnalytics>(id, transaction);

    public async Task<IEnumerable<SessionAnalytics>> GetAllSessionsAnalyticsAsync(IDbTransaction? transaction = null) =>
        await GetListAsync<SessionAnalytics>(transaction);

    public async Task<IEnumerable<SessionAnalytics>> GetSessionsAnalyticsByDepartmentIdAsync(int departmentId, IDbTransaction? transaction = null)
    {
        await using var connection = new NpgsqlConnection(ConnectionString);
        return await connection.GetListAsync<SessionAnalytics>(
            "WHERE \"DepartmentId\" = @Id",
            new { Id = departmentId }, transaction) ?? new List<SessionAnalytics>();
    }
    
    public async Task<IEnumerable<SessionAnalytics>> GetSessionsAnalyticsByDateAsync(DateOnly date, IDbTransaction? transaction = null)
    {
        await using var connection = new NpgsqlConnection(ConnectionString);
        return await connection.GetListAsync<SessionAnalytics>(
            "WHERE \"SessionDate\" = @Date",
            new { Date = date }, transaction) ?? new List<SessionAnalytics>();
    }
    
    public async Task<IEnumerable<SessionAnalytics>> GetSessionsAnalyticsByDepartmentIdAndDateAsync(int departmentId, DateOnly date, IDbTransaction? transaction = null)
    {
        await using var connection = new NpgsqlConnection(ConnectionString);
        return await connection.GetListAsync<SessionAnalytics>(
            "WHERE \"DepartmentId\" = @Id AND \"SessionDate\" = @Date",
            new { Id = departmentId, Date = date }, transaction) ?? new List<SessionAnalytics>();
    }
}