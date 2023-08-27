using Domain.Models.MessageSources;

namespace Application.DTO.Messages;

public record EmailMessageDto(int Id, string Text, int SenderSourceId, int RecipientSourceId, bool Checked, DateTime Date) 
    : MessageDto(Id, Text, SenderSourceId, RecipientSourceId, Checked, SourceType.Email, Date);