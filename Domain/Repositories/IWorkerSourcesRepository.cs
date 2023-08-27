using System.Data;
using Domain.Models;

namespace Domain.Repositories;

public interface IWorkerSourcesRepository : IBaseRepository
{
    Task<int?> InsertWorkerSourcesAsync(WorkerSources workerSources, IDbTransaction? transaction = null);
    Task<int> DeleteWorkerSourcesAsync(int id, IDbTransaction? transaction = null);
    Task DeleteAllWorkerSourcesAsync(IDbTransaction? transaction = null);
    Task<int> UpdateWorkerSourcesAsync(WorkerSources workerSources, IDbTransaction? transaction = null);
    Task<IEnumerable<WorkerSources>> GetAllWorkerSourcesAsync(IDbTransaction? transaction = null);
    Task<WorkerSources?> GetWorkerSourcesByWorkerIdAsync(int workerId, IDbTransaction? transaction = null);
}