using System.Data;
using Dapper;
using Domain.Models;
using Npgsql;

namespace Domain.Repositories;

public interface ISourceAnalyticsRepository : IBaseRepository
{ 
    Task<int?> InsertSourceAnalyticsAsync(SourceAnalytics sourceAnalytics, IDbTransaction? transaction = null);
    Task<int> DeleteSourceAnalyticsAsync(int id, IDbTransaction? transaction = null);
    Task DeleteAllSourcesAnalyticsAsync(IDbTransaction? transaction = null);
    Task<int> UpdateSourceAnalyticsAsync(SourceAnalytics sourceAnalytics, IDbTransaction? transaction = null);
    Task<SourceAnalytics> GetSourceAnalyticsAsync(int id, IDbTransaction? transaction = null);
    Task<SourceAnalytics?> GetSourceAnalyticsByMessageSourceIdAndDateAsync(int messageSourceId, DateOnly date, IDbTransaction? transaction = null);
    Task<IEnumerable<SourceAnalytics>> GetSourcesAnalyticsByMessageSourceIdAsync(int messageSourceId, IDbTransaction? transaction = null);
    Task<IEnumerable<SourceAnalytics>> GetSourcesAnalyticsByDepartmentIdAndDateAsync(int departmentId, DateOnly date, IDbTransaction? transaction = null);
    Task<IEnumerable<SourceAnalytics>> GetSourcesAnalyticsByDateAsync(DateOnly date, IDbTransaction? transaction = null);
    Task<IEnumerable<SourceAnalytics>> GetAllSourcesAnalyticsAsync(IDbTransaction? transaction = null);
}