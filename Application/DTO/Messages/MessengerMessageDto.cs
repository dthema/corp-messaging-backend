using Domain.Models.MessageSources;

namespace Application.DTO.Messages;

public record MessengerMessageDto(int Id, string Text, int SenderSourceId, int RecipientSourceId, bool Checked, DateTime Date) 
    : MessageDto(Id, Text, SenderSourceId, RecipientSourceId, Checked, SourceType.Messenger, Date);