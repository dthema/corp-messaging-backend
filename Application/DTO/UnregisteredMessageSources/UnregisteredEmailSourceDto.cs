using Domain.Models.Messages;
using Domain.Models.MessageSources;

namespace Application.DTO.UnregisteredMessageSources;

public record UnregisteredEmailSourceDto(SourceType SourceType, string Email) : UnregisteredMessageSourceDto(SourceType.Email);