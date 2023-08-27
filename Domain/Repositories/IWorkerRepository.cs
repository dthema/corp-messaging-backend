using System.Data;
using Dapper;
using Domain.Models;
using Domain.Models.Workers;
using Npgsql;

namespace Domain.Repositories;

public interface IWorkerRepository : IBaseRepository
{ 
    Task<int?> InsertWorkerAsync<T>(T worker, IDbTransaction? transaction = null) where T : Worker, new();
    Task<int> DeleteWorkerAsync(int id, IDbTransaction? transaction = null);
    Task DeleteAllWorkersAsync(IDbTransaction? transaction = null);
    Task<int> UpdateWorkerAsync<T>(T worker, IDbTransaction? transaction = null) where T : Worker, new();
    Task<Worker?> GetWorkerAsync(int id, IDbTransaction? transaction = null);
    Task<IEnumerable<Worker>> GetWorkersByRoleAsync(Role role, IDbTransaction? transaction = null);
    Task<IEnumerable<T>> GetWorkersAsync<T>(IDbTransaction? transaction = null) where T : Worker, new();
    Task<IEnumerable<Worker>> GetWorkersByDepartmentIdAsync(int departmentId, IDbTransaction? transaction = null);
    Task<IEnumerable<T>> GetWorkersByDepartmentIdAsync<T>(int departmentId, IDbTransaction? transaction = null) where T : Worker, IDepartmentalWorker, new();
    Task<IEnumerable<Worker>> GetAllWorkersAsync(IDbTransaction? transaction = null);
}