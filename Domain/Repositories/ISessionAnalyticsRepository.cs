using System.Data;
using Dapper;
using Domain.Models;
using Npgsql;

namespace Domain.Repositories;

public interface ISessionAnalyticsRepository : IBaseRepository
{
    Task<int?> InsertSessionAnalyticsAsync(SessionAnalytics sessionAnalytics, IDbTransaction? transaction = null);
    Task<int> DeleteSessionAnalyticsAsync(int id, IDbTransaction? transaction = null);
    Task DeleteAllSessionsAnalyticsAsync(IDbTransaction? transaction = null);
    Task<int> UpdateSessionAnalyticsAsync(SessionAnalytics sessionAnalytics, IDbTransaction? transaction = null);
    Task<SessionAnalytics> GetSessionAnalyticsAsync(int id, IDbTransaction? transaction = null);
    Task<IEnumerable<SessionAnalytics>> GetAllSessionsAnalyticsAsync(IDbTransaction? transaction = null);
    Task<IEnumerable<SessionAnalytics>> GetSessionsAnalyticsByDepartmentIdAsync(int departmentId, IDbTransaction? transaction = null);
    Task<IEnumerable<SessionAnalytics>> GetSessionsAnalyticsByDateAsync(DateOnly date, IDbTransaction? transaction = null);
    Task<IEnumerable<SessionAnalytics>> GetSessionsAnalyticsByDepartmentIdAndDateAsync(int departmentId, DateOnly date, IDbTransaction? transaction = null);
}