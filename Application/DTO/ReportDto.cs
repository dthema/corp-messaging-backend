namespace Application.DTO;

public record ReportDto(int Id, int CheckedMessages, IEnumerable<SourceAnalyticsDto> SourcesAnalytics);