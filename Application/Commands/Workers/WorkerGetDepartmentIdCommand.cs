using System.Data;
using Domain.Models;
using Domain.Models.Workers;
using Domain.Repositories;

namespace Application.Commands.Workers;

public class WorkerGetDepartmentIdCommand : WorkerCommand
{
    public WorkerGetDepartmentIdCommand(IWorkerRepository workerRepository, IDepartmentRepository departmentRepository, IDbTransaction? transaction = null)
        : base(workerRepository, departmentRepository, transaction) { }

    public override async Task<int?> Execute(Employee employee)
    {
        Department department = await _departmentalWorker.GetDepartmentAsync(employee.DepartmentId) ?? throw new Exception();
        return department.Id;
    }

    public override async Task<int?> Execute(Manager manager)
    {
        Department department = await _departmentalWorker.GetDepartmentAsync(manager.DepartmentId) ?? throw new Exception();
        return department.Id;
    }

    public override async Task<int?> Execute(Chief chief)
    {
        return 0;
    }
}