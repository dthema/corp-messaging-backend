using Domain.Models.Messages;
using Domain.Models.MessageSources;

namespace Application.DTO.UnregisteredMessageSources;

public abstract record UnregisteredMessageSourceDto(SourceType SourceType);