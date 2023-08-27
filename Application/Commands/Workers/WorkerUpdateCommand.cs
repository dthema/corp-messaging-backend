using System.Data;
using Domain.Models;
using Domain.Models.Workers;
using Domain.Repositories;

namespace Application.Commands.Workers;

public class WorkerUpdateCommand : WorkerCommand
{
    private readonly int _workerId;

    public WorkerUpdateCommand(IWorkerRepository workerRepository, IDepartmentRepository departmentRepository, int workerId, IDbTransaction? transaction = null)
        : base(workerRepository, departmentRepository, transaction)
    {
        _workerId = workerId;
    }

    public override async Task<int?> Execute(Employee employee)
    {
        employee.Id = _workerId;
        Department department = await _departmentalWorker.GetDepartmentAsync(employee.DepartmentId) ?? throw new Exception();
        return await _workerRepository.UpdateWorkerAsync(employee);
    }

    public override async Task<int?> Execute(Manager manager)
    {
        manager.Id = _workerId;
        Department department = await _departmentalWorker.GetDepartmentAsync(manager.DepartmentId) ?? throw new Exception();
        return await _workerRepository.UpdateWorkerAsync(manager);
    }

    public override async Task<int?> Execute(Chief chief)
    {
        chief.Id = _workerId;
        return await _workerRepository.UpdateWorkerAsync(chief);
    }
}