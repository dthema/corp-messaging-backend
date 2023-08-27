using System.Data;
using Domain.Models;
using Domain.Models.Workers;
using Domain.Repositories;

namespace Application.Commands.Workers;

public class WorkerInsertCommand : WorkerCommand
{
    public WorkerInsertCommand(IWorkerRepository workerRepository, IDepartmentRepository departmentRepository, IDbTransaction? transaction = null)
        : base(workerRepository, departmentRepository, transaction) { }

    public override async Task<int?> Execute(Employee employee)
    {
        Department department = await _departmentalWorker.GetDepartmentAsync(employee.DepartmentId) ?? throw new Exception();
        return await _workerRepository.InsertWorkerAsync(employee);
    }

    public override async Task<int?> Execute(Manager manager)
    {
        Department department = await _departmentalWorker.GetDepartmentAsync(manager.DepartmentId) ?? throw new Exception();
        return await _workerRepository.InsertWorkerAsync(manager);
    }

    public override async Task<int?> Execute(Chief chief)
    {
        return await _workerRepository.InsertWorkerAsync(chief);
    }
}