using Application.DTO.Messages;
using Domain.Models.MessageSources;

namespace Application.DTO.MessageSources;

public record MessengerSourceDto(
    int Id,
    string MessengerName,
    string Username,
    IReadOnlyCollection<MessageDto> IncomingMessages,
    IReadOnlyCollection<MessageDto> OutgoingMessages)
    : MessageSourceDto(Id, SourceType.Messenger, IncomingMessages, OutgoingMessages);