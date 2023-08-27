namespace Presentation.DTO;

public record MessageSendDto(int SenderSourceId, int RecipientSourceId, string Text);