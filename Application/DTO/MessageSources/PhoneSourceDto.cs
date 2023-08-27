using Application.DTO.Messages;
using Domain.Models.MessageSources;

namespace Application.DTO.MessageSources;

public record PhoneSourceDto(
    int Id,
    string PhoneNumber,
    IReadOnlyCollection<MessageDto> IncomingMessages,
    IReadOnlyCollection<MessageDto> OutgoingMessages)
    : MessageSourceDto(Id, SourceType.Phone, IncomingMessages, OutgoingMessages);