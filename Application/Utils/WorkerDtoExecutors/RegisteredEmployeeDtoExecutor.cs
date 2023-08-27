using Application.Commands.Workers;
using Application.DTO.UnregisteredWorkers;
using Application.DTO.Workers;
using Domain.Models.Workers;

namespace Application.Utils.WorkerDtoExecutors;

public class RegisteredEmployeeDtoExecutor : IWorkerDtoExecutor
{
    private IWorkerDtoExecutor? _nextFactory;
    private readonly IWorkerCommand _command;

    public RegisteredEmployeeDtoExecutor(IWorkerCommand command)
    {
        _command = command;
    }

    public async Task<int?> DoWorkerDtoCommand(UnregisteredWorkerDto workerDto)
    {
        if (workerDto is EmployeeDto employeeDto)
            return await _command.Execute(new Employee
            {
                Id = employeeDto.Id,
                Firstname = employeeDto.Firstname,
                Lastname = employeeDto.Lastname,
                DepartmentId = employeeDto.DepartmentId,
                Role = employeeDto.Role
            });
        if (_nextFactory is not null)
            return await _nextFactory.DoWorkerDtoCommand(workerDto);
        throw new Exception();
    }

    public IWorkerDtoExecutor AddNext(IWorkerDtoExecutor workerDtoExecutor)
    {
        ArgumentNullException.ThrowIfNull(workerDtoExecutor);
        _nextFactory = workerDtoExecutor;
        return workerDtoExecutor;
    }
}