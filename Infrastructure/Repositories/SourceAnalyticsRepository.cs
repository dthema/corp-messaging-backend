using System.Data;
using Dapper;
using Domain.Models;
using Domain.Models.MessageSources;
using Domain.Models.Workers;
using Domain.Repositories;
using Npgsql;

namespace Infrastructure.Repositories;

public class SourceAnalyticsRepository : BaseRepository, ISourceAnalyticsRepository
{
    public SourceAnalyticsRepository()
    {
        SimpleCRUD.SetDialect(SimpleCRUD.Dialect.PostgreSQL);
    }
    
    public async Task<int?> InsertSourceAnalyticsAsync(SourceAnalytics sourceAnalytics, IDbTransaction? transaction = null) =>
        await InsertAsync(sourceAnalytics, transaction);

    public async Task<int> DeleteSourceAnalyticsAsync(int id, IDbTransaction? transaction = null) =>
        await DeleteAsync<SourceAnalytics>(id, transaction);

    public async Task DeleteAllSourcesAnalyticsAsync(IDbTransaction? transaction = null) =>
        await DeleteAllAsync<SourceAnalytics>(transaction);

    public async Task<int> UpdateSourceAnalyticsAsync(SourceAnalytics sourceAnalytics, IDbTransaction? transaction = null) =>
        await UpdateAsync(sourceAnalytics, transaction);

    public async Task<SourceAnalytics> GetSourceAnalyticsAsync(int id, IDbTransaction? transaction = null) =>
        await GetAsync<SourceAnalytics>(id, transaction);

    public async Task<IEnumerable<SourceAnalytics>> GetAllSourcesAnalyticsAsync(IDbTransaction? transaction = null) => 
        await GetListAsync<SourceAnalytics>(transaction);
    
    public async Task<IEnumerable<SourceAnalytics>> GetSourcesAnalyticsByMessageSourceIdAsync(int messageSourceId, IDbTransaction? transaction = null)
    {
        await using var connection = new NpgsqlConnection(ConnectionString);
        return await connection.GetListAsync<SourceAnalytics>(
            "WHERE \"MessageSourceId\" = @Id",
            new { Id = messageSourceId }, transaction);
    }

    public async Task<SourceAnalytics?> GetSourceAnalyticsByMessageSourceIdAndDateAsync(int messageSourceId, DateOnly date, IDbTransaction? transaction = null)
    {
        await using var connection = new NpgsqlConnection(ConnectionString);
        var sourcesAnalytics = await connection.GetListAsync<SourceAnalytics>(
            "WHERE \"MessageSourceId\" = @Id AND \"SessionDate\" = @Date",
            new { Id = messageSourceId, Date = date }, transaction);
        return sourcesAnalytics.FirstOrDefault(analytics => analytics.MessageSourceId == messageSourceId);
    }

    public async Task<SourceAnalytics?> GetSourcesAnalyticsByMessageSourceIdAndDateAsync(int messageSourceId, DateOnly date, IDbTransaction? transaction = null)
    {
        await using var connection = new NpgsqlConnection(ConnectionString);
        var sourcesAnalytics = await connection.GetListAsync<SourceAnalytics>(
            "WHERE \"MessageSourceId\" = @Id AND \"SessionDate\" = @Date",
            new { Id = messageSourceId, Date = date }, transaction);
        return sourcesAnalytics.FirstOrDefault(analytics => analytics.MessageSourceId == messageSourceId);
    }
    
    public async Task<IEnumerable<SourceAnalytics>> GetSourcesAnalyticsByDepartmentIdAndDateAsync(int departmentId, DateOnly date, IDbTransaction? transaction = null)
    {
        await using var connection = new NpgsqlConnection(ConnectionString);
        
        var workerRepository = new WorkerRepository();
        var workers = await workerRepository.GetWorkersByDepartmentIdAsync(departmentId);
        
        IEnumerable<MessageSource> sources = workers.SelectMany(x => x.MessageSources).ToList();
        
        var sourcesAnalytics = new List<SourceAnalytics>();
        foreach (MessageSource messageSource in sources)
        {
            var sourceAnalytics = await GetSourcesAnalyticsByMessageSourceIdAndDateAsync(messageSource.Id, date, transaction);
            if (sourceAnalytics is not null)
                sourcesAnalytics.Add(sourceAnalytics);
        }

        return sourcesAnalytics;
    }

    public async Task<IEnumerable<SourceAnalytics>> GetSourcesAnalyticsByDateAsync(DateOnly date, IDbTransaction? transaction = null)
    {
        await using var connection = new NpgsqlConnection(ConnectionString);
        return await connection.GetListAsync<SourceAnalytics>(
            "WHERE \"SessionDate\" = @Date",
            new { Date = date }, transaction);
    }
}