using Domain.Models.MessageSources;

namespace Application.DTO.Messages;

public record MessageDto(int Id, string Text, int SenderSourceId, int RecipientSourceId, bool Checked, SourceType SourceType, DateTime Date);