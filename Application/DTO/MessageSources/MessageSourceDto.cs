using Application.DTO.Messages;
using Application.DTO.UnregisteredMessageSources;
using Domain.Models.Messages;
using Domain.Models.MessageSources;

namespace Application.DTO.MessageSources;

public abstract record MessageSourceDto(
    int Id,
    SourceType SourceType,
    IReadOnlyCollection<MessageDto> IncomingMessages,
    IReadOnlyCollection<MessageDto> OutgoingMessages)
    : UnregisteredMessageSourceDto(SourceType);