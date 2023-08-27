using System.Data;
using System.Data.Common;
using Dapper;
using Domain.Models;
using Domain.Models.MessageSources;
using Domain.Models.Workers;
using Domain.Repositories;
using Npgsql;

namespace Infrastructure.Repositories;

public class WorkerRepository : BaseRepository, IWorkerRepository
{ 
    private const string WorkersTableName = "\"Workers\"";

    public WorkerRepository()
    {
        SimpleCRUD.SetDialect(SimpleCRUD.Dialect.PostgreSQL);
    }
    
    public async Task<int?> InsertWorkerAsync<T>(T worker, IDbTransaction? transaction = null) where T : Worker, new() => 
        await InsertAsync(worker, transaction);

    public async Task<int> DeleteWorkerAsync(int id, IDbTransaction? transaction = null) =>
        await DeleteAsync<Worker>(id, transaction);

    public async Task DeleteAllWorkersAsync(IDbTransaction? transaction = null) =>
        await DeleteAllAsync<Worker>(transaction);

    public async Task<int> UpdateWorkerAsync<T>(T worker, IDbTransaction? transaction = null) where T : Worker, new() =>
        await UpdateAsync(worker, transaction);

    public async Task<Worker?> GetWorkerAsync(int id, IDbTransaction? transaction = null)
    {
        await using var connection = new NpgsqlConnection(ConnectionString);
        var workers = await GetWorkersByQueryAsync(
            connection,
            $"SELECT * FROM {WorkersTableName} WHERE \"Id\" = @Id",
            new {Id = id}, transaction);
        return workers.FirstOrDefault(x => x.Id == id);
    }

    public async Task<IEnumerable<Worker>> GetWorkersByRoleAsync(Role role, IDbTransaction? transaction = null)
    {
        await using var connection = new NpgsqlConnection(ConnectionString);
        return await GetWorkersByQueryAsync(
            connection,
            $"SELECT * FROM {WorkersTableName} WHERE \"Role\" = @Role",
            new {Role = role}, transaction);
    }

    public async Task<IEnumerable<T>> GetWorkersAsync<T>(IDbTransaction? transaction = null) where T : Worker, new()
    {
        await using var connection = new NpgsqlConnection(ConnectionString);
        var model = new T();
        var workers = await connection.GetListAsync<T>(
            "WHERE \"Role\" = @Role",
            new { Role = model.Role }, transaction) ?? new List<T>();
        workers = workers.ToList();
        foreach (T worker in workers)
            await FillWorkerSources(worker);
        return workers;
    }

    public async Task<IEnumerable<Worker>> GetWorkersByDepartmentIdAsync(int departmentId, IDbTransaction? transaction = null)
    {
        await using var connection = new NpgsqlConnection(ConnectionString);
        return await GetWorkersByQueryAsync(
            connection,
            $"SELECT * FROM {WorkersTableName} WHERE \"DepartmentId\" = @DepartmentId",
            new {DepartmentId = departmentId}, transaction);
    }

    public async Task<IEnumerable<T>> GetWorkersByDepartmentIdAsync<T>(int departmentId, IDbTransaction? transaction = null)
        where T : Worker, IDepartmentalWorker, new()
    {
        await using var connection = new NpgsqlConnection(ConnectionString);
        var model = new T();
        var workers = await connection.GetListAsync<T>(
            "WHERE \"DepartmentId\" = @Id AND \"Role\" = @Role",
            new { Id = departmentId, Role = model.Role}, transaction) ?? new List<T>();
        workers = workers.ToList();
        foreach (T worker in workers)
            await FillWorkerSources(worker);
        return workers;
    }
    
    public async Task<IEnumerable<Worker>> GetAllWorkersAsync(IDbTransaction? transaction = null)
    {
        await using var connection = new NpgsqlConnection(ConnectionString);
        return await GetWorkersByQueryAsync(connection, $"SELECT * FROM {WorkersTableName}", transaction: transaction);
    }
    
    private async Task<IEnumerable<Worker>> GetWorkersByQueryAsync(DbConnection connection, string query, object? parameters = null, IDbTransaction? transaction = null)
    {
        var workers = new List<Worker>();

        using var reader = await connection.ExecuteReaderAsync(query, parameters, transaction);
        var employeeParser = reader.GetRowParser<Employee>();
        var managerParser = reader.GetRowParser<Manager>();
        var chiefParser = reader.GetRowParser<Chief>();

        var sourcesRepository = new MessageSourceRepository();
        while(reader.Read())
        {
            var discriminator = (Role)reader.GetInt32(reader.GetOrdinal(nameof(Role)));
            Worker? worker = null;
            switch(discriminator)
            {
                case Role.Employee:
                    worker = employeeParser(reader);
                    break;
                case Role.Manager:
                    worker = managerParser(reader);
                    break;
                case Role.Chief:
                    worker = chiefParser(reader);
                    break;
            }

            if (worker is null) continue;
            await FillWorkerSources(worker, sourcesRepository);
            workers.Add(worker);
        }

        return workers;
    }

    private async Task FillWorkerSources(Worker worker, MessageSourceRepository? sourcesRepository = null)
    {
        var repository = sourcesRepository ?? new MessageSourceRepository();
        var sources = await repository.GetMessageSourcesByWorkerIdAsync(worker.Id) ?? new List<MessageSource>();
        worker.MessageSources = sources.ToList();
    }
}