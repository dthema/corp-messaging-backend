using Application.DTO.UnregisteredMessageSources;
using Application.DTO.UnregisteredWorkers;

namespace Application.Utils.MessageSourceDtoExecutors;

public interface IMessageSourceDtoExecutor
{
    Task<int?> DoMessageSourceDtoCommand(UnregisteredMessageSourceDto messageSourceDto);
    IMessageSourceDtoExecutor AddNext(IMessageSourceDtoExecutor messageSourceDtoExecutor);
}