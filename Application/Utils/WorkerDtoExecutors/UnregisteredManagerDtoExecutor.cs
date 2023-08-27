using Application.Commands.Workers;
using Application.DTO.UnregisteredWorkers;
using Domain.Models.Workers;

namespace Application.Utils.WorkerDtoExecutors;

public class UnregisteredManagerDtoExecutor : IWorkerDtoExecutor
{
    private IWorkerDtoExecutor? _nextFactory;
    private readonly IWorkerCommand _command;

    public UnregisteredManagerDtoExecutor(IWorkerCommand command)
    {
        _command = command;
    }

    public async Task<int?> DoWorkerDtoCommand(UnregisteredWorkerDto workerDto)
    {
        if (workerDto is UnregisteredManagerDto unregisteredManagerDto)
            return await _command.Execute(new Manager
            {
                Firstname = unregisteredManagerDto.Firstname,
                Lastname = unregisteredManagerDto.Lastname,
                DepartmentId = unregisteredManagerDto.DepartmentId,
                Role = unregisteredManagerDto.Role
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