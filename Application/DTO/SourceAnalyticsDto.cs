using Application.DTO.MessageSources;

namespace Application.DTO;

public record SourceAnalyticsDto(int Id, MessageSourceDto SourceDto, int MessagesCount);