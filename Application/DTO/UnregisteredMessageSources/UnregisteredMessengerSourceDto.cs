using Domain.Models.Messages;
using Domain.Models.MessageSources;

namespace Application.DTO.UnregisteredMessageSources;

public record UnregisteredMessengerSourceDto(SourceType SourceType, string MessengerName, string Username) : UnregisteredMessageSourceDto(SourceType.Messenger);