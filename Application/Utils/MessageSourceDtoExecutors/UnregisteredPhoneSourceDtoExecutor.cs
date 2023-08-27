using Application.Commands.MessageSources;
using Application.Commands.Workers;
using Application.DTO.MessageSources;
using Application.DTO.UnregisteredMessageSources;
using Application.DTO.UnregisteredWorkers;
using Application.DTO.Workers;
using Application.Utils.WorkerDtoExecutors;
using Domain.Models.MessageSources;
using Domain.Models.Workers;

namespace Application.Utils.MessageSourceDtoExecutors;

public class UnregisteredPhoneSourceDtoExecutor : IMessageSourceDtoExecutor
{
    private IMessageSourceDtoExecutor? _nextFactory;
    private readonly IMessageSourceCommand _command;

    public UnregisteredPhoneSourceDtoExecutor(IMessageSourceCommand command)
    {
        _command = command;
    }

    public async Task<int?> DoMessageSourceDtoCommand(UnregisteredMessageSourceDto messageSourceDto)
    {
        if (messageSourceDto is UnregisteredPhoneSourceDto phoneSourceDto)
            return await _command.Execute(new PhoneSource
            {
                PhoneNumber = phoneSourceDto.PhoneNumber,
                SourceType = phoneSourceDto.SourceType
            });
        if (_nextFactory is not null)
            return await _nextFactory.DoMessageSourceDtoCommand(messageSourceDto);
        throw new Exception();
    }

    public IMessageSourceDtoExecutor AddNext(IMessageSourceDtoExecutor messageSourceDtoExecutor)
    {
        ArgumentNullException.ThrowIfNull(messageSourceDtoExecutor);
        _nextFactory = messageSourceDtoExecutor;
        return messageSourceDtoExecutor;
    }
}