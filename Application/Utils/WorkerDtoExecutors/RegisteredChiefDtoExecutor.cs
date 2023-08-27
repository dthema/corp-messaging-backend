using Application.Commands.Workers;
using Application.DTO.UnregisteredWorkers;
using Application.DTO.Workers;
using Domain.Models.Workers;

namespace Application.Utils.WorkerDtoExecutors;

public class RegisteredChiefDtoExecutor : IWorkerDtoExecutor
{
    private IWorkerDtoExecutor? _nextFactory;
    private readonly IWorkerCommand _command;

    public RegisteredChiefDtoExecutor(IWorkerCommand command)
    {
        _command = command;
    }

    public async Task<int?> DoWorkerDtoCommand(UnregisteredWorkerDto workerDto)
    {
        if (workerDto is ChiefDto chiefDto)
            return await _command.Execute(new Chief
            {
                Id = chiefDto.Id,
                Firstname = chiefDto.Firstname,
                Lastname = chiefDto.Lastname,
                Role = chiefDto.Role,
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