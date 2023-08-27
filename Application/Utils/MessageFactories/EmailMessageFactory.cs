using Application.DTO.Messages;
using Application.DTO.MessageSources;
using Application.Utils.MessageSourceFactories;
using Domain.Models.Messages;
using Domain.Models.MessageSources;

namespace Application.Utils.MessageFactories;

public class EmailMessageFactory : IMessageFactory
{
    private IMessageFactory? _nextFactory;
    
    public Message CreateMessage(MessageSource senderSource, MessageSource recipientSource, string text)
    {
        if (senderSource.SourceType != recipientSource.SourceType)
            throw new Exception();
        if (senderSource is EmailSource)
            return new EmailMessage
            {
                SenderSourceId = senderSource.Id,
                RecipientSourceId = recipientSource.Id,
                Text = text,
                Checked = false,
                Date = DateTime.Now
            };
        if (_nextFactory is not null)
            return _nextFactory.CreateMessage(senderSource, recipientSource, text);
        throw new Exception();
    }

    public IMessageFactory AddNext(IMessageFactory messageFactory)
    {
        ArgumentNullException.ThrowIfNull(messageFactory);
        _nextFactory = messageFactory;
        return messageFactory;
    }
}