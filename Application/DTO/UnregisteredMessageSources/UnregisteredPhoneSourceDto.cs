using Domain.Models.Messages;
using Domain.Models.MessageSources;

namespace Application.DTO.UnregisteredMessageSources;

public record UnregisteredPhoneSourceDto(SourceType SourceType, string PhoneNumber) : UnregisteredMessageSourceDto(SourceType.Phone);