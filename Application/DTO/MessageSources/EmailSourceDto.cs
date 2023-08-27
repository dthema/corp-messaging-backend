using Application.DTO.Messages;
using Domain.Models.Messages;
using Domain.Models.MessageSources;

namespace Application.DTO.MessageSources;

public record EmailSourceDto(
    int Id,
    string Email,
    IReadOnlyCollection<MessageDto> IncomingMessages,
    IReadOnlyCollection<MessageDto> OutgoingMessages)
    : MessageSourceDto(Id, SourceType.Email, IncomingMessages, OutgoingMessages);