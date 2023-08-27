using Application.DTO.Messages;

namespace Application.Services.Interfaces;

public interface IMessageService
{ 
    Task<MessageDto> SendMessage(int senderSourceId, int recipientSourceId, string text);
    Task CheckMessage(int sessionId, int messageId);
}