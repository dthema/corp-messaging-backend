using System.Data;
using Dapper;
using Domain.Models;
using Domain.Repositories;
using Npgsql;

namespace Infrastructure.Repositories;

public class WorkerSourcesRepository : BaseRepository, IWorkerSourcesRepository
{
    private const string WorkersSourcesTableName = "\"WorkersSourcesMapping\"";

    public WorkerSourcesRepository()
    {
        SimpleCRUD.SetDialect(SimpleCRUD.Dialect.PostgreSQL);
    }
    
    public async Task<int?> InsertWorkerSourcesAsync(WorkerSources workerSources, IDbTransaction? transaction = null) =>
        await InsertAsync(workerSources, transaction);

    public async Task<int> DeleteWorkerSourcesAsync(int id, IDbTransaction? transaction = null) =>
        await DeleteAsync<WorkerSources>(id, transaction);

    public async Task DeleteAllWorkerSourcesAsync(IDbTransaction? transaction = null) =>
        await DeleteAllAsync<WorkerSources>(transaction);

    public async Task<int> UpdateWorkerSourcesAsync(WorkerSources workerSources, IDbTransaction? transaction = null) => 
        await UpdateAsync(workerSources, transaction);

    public async Task<IEnumerable<WorkerSources>> GetAllWorkerSourcesAsync(IDbTransaction? transaction = null) => 
        await GetListAsync<WorkerSources>(transaction);

    public async Task<WorkerSources?> GetWorkerSourcesByWorkerIdAsync(int workerId, IDbTransaction? transaction = null)
    {
        await using var connection = new NpgsqlConnection(ConnectionString);
        var workerSources = await connection.GetListAsync<WorkerSources>(
            "WHERE \"WorkerId\" = @Id",
            new { Id = workerId }, transaction);
        return workerSources.FirstOrDefault(x => x.WorkerId == workerId);
    }
}