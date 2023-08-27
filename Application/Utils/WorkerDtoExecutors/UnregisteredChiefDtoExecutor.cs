using Application.Commands.Workers;
using Application.DTO.UnregisteredWorkers;
using Domain.Models.Workers;

namespace Application.Utils.WorkerDtoExecutors;

public class UnregisteredChiefDtoExecutor : IWorkerDtoExecutor
{
    private IWorkerDtoExecutor? _nextFactory;
    private readonly IWorkerCommand _command;

    public UnregisteredChiefDtoExecutor(IWorkerCommand command)
    {
        _command = command;
    }

    public async Task<int?> DoWorkerDtoCommand(UnregisteredWorkerDto workerDto)
    {
        if (workerDto is UnregisteredChiefDto unregisteredChiefDto)
            return await _command.Execute(new Chief
            {
                Firstname = unregisteredChiefDto.Firstname,
                Lastname = unregisteredChiefDto.Lastname,
                Role = unregisteredChiefDto.Role
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