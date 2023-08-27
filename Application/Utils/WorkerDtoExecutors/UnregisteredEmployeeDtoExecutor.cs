using Application.Commands.Workers;
using Application.DTO.UnregisteredWorkers;
using Domain.Models.Workers;

namespace Application.Utils.WorkerDtoExecutors;

public class UnregisteredEmployeeDtoExecutor : IWorkerDtoExecutor
{
    private IWorkerDtoExecutor? _nextFactory;
    private readonly IWorkerCommand _command;

    public UnregisteredEmployeeDtoExecutor(IWorkerCommand command)
    {
        _command = command;
    }

    public async Task<int?> DoWorkerDtoCommand(UnregisteredWorkerDto workerDto)
    {
        if (workerDto is UnregisteredEmployeeDto unregisteredEmployeeDto)
            return await _command.Execute(new Employee
            {
                Firstname = unregisteredEmployeeDto.Firstname,
                Lastname = unregisteredEmployeeDto.Lastname,
                DepartmentId = unregisteredEmployeeDto.DepartmentId,
                Role = unregisteredEmployeeDto.Role
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