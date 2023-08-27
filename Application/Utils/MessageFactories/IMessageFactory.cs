using Application.DTO.MessageSources;
using Domain.Models.Messages;
using Domain.Models.MessageSources;

namespace Application.Utils.MessageFactories;

public interface IMessageFactory
{
    Message CreateMessage(MessageSource senderSource, MessageSource recipientSource, string text);
    IMessageFactory AddNext(IMessageFactory messageFactory);
}