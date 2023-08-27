using System.Data;
using Application.Utils.WorkerFactories;
using Domain.Models.Workers;
using Domain.Repositories;

namespace Application.Commands.Workers;

public abstract class WorkerCommand : IWorkerCommand
{
    protected readonly IWorkerRepository _workerRepository;
    protected readonly IDepartmentRepository _departmentalWorker;
    protected IDbTransaction? _transaction;

    public WorkerCommand(IWorkerRepository workerRepository, IDepartmentRepository departmentRepository, IDbTransaction? transaction = null)
    {
        ArgumentNullException.ThrowIfNull(workerRepository);
        _workerRepository = workerRepository;
        _departmentalWorker = departmentRepository;
        _transaction = transaction;
    }
    
    public abstract Task<int?> Execute(Employee employee);
    public abstract Task<int?> Execute(Manager manager);
    public abstract Task<int?> Execute(Chief chief);
}