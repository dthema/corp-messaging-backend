using Application.DTO.Messages;
using Application.DTO.MessageSources;
using Domain.Models.MessageSources;

namespace Application.Utils.MessageSourceFactories;

public class EmailSourceFactory : IMessageSourceFactory
{
    private IMessageSourceFactory? _nextFactory;

    public MessageSourceDto CreateMessageSourceDto(MessageSource messageSource)
    {
        if (messageSource is EmailSource emailSource)
            return new EmailSourceDto(emailSource.Id, emailSource.Email, 
                messageSource.IncomingMessages.Select(message => 
                    new MessageDto(message.Id, message.Text, message.SenderSourceId, message.RecipientSourceId, message.Checked, message.SourceType, message.Date)).ToList(), 
                messageSource.OutgoingMessages.Select(message => 
                    new MessageDto(message.Id, message.Text, message.SenderSourceId, message.RecipientSourceId, message.Checked, message.SourceType, message.Date)).ToList()
                );
        if (_nextFactory is not null)
            return _nextFactory.CreateMessageSourceDto(messageSource);
        throw new Exception();
    }

    public IMessageSourceFactory AddNext(IMessageSourceFactory messageSourceFactory)
    {
        ArgumentNullException.ThrowIfNull(messageSourceFactory);
        _nextFactory = messageSourceFactory;
        return messageSourceFactory;
    }
}