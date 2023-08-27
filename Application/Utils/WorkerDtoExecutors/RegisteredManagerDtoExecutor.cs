using Application.Commands.Workers;
using Application.DTO.UnregisteredWorkers;
using Application.DTO.Workers;
using Domain.Models.Workers;

namespace Application.Utils.WorkerDtoExecutors;

public class RegisteredManagerDtoExecutor : IWorkerDtoExecutor
{
    private IWorkerDtoExecutor? _nextFactory;
    private readonly IWorkerCommand _command;

    public RegisteredManagerDtoExecutor(IWorkerCommand command)
    {
        _command = command;
    }

    public async Task<int?> DoWorkerDtoCommand(UnregisteredWorkerDto workerDto)
    {
        if (workerDto is ManagerDto managerDto)
            return await _command.Execute(new Employee
            {
                Id = managerDto.Id,
                Firstname = managerDto.Firstname,
                Lastname = managerDto.Lastname,
                DepartmentId = managerDto.DepartmentId,
                Role = managerDto.Role
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