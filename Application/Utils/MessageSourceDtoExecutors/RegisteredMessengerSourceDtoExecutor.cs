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

public class RegisteredMessengerSourceDtoExecutor : IMessageSourceDtoExecutor
{
    private IMessageSourceDtoExecutor? _nextFactory;
    private readonly IMessageSourceCommand _command;

    public RegisteredMessengerSourceDtoExecutor(IMessageSourceCommand command)
    {
        _command = command;
    }

    public async Task<int?> DoMessageSourceDtoCommand(UnregisteredMessageSourceDto messageSourceDto)
    {
        if (messageSourceDto is MessengerSourceDto messengerSourceDto)
            return await _command.Execute(new MessengerSource
            {
                Id = messengerSourceDto.Id,
                MessengerName = messengerSourceDto.MessengerName,
                Username = messengerSourceDto.Username,
                SourceType = messengerSourceDto.SourceType
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