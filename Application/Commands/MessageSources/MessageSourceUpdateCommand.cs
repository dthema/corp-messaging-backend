using System.Data;
using Domain.Models.MessageSources;
using Domain.Repositories;

namespace Application.Commands.MessageSources;

public class MessageSourceUpdateCommand : MessageSourceCommand
{
    private readonly int _sourceId;

    public MessageSourceUpdateCommand(IMessageSourceRepository messageSourceRepository, int sourceId,
        IDbTransaction? transaction = null)
        : base(messageSourceRepository, transaction)
    {
        _sourceId = sourceId;
    }

    public override async Task<int?> Execute(EmailSource emailSource)
    {
        emailSource.Id = _sourceId;
        return await _messageSourceRepository.UpdateMessageSourceAsync(emailSource, _transaction);
    }

    public override async Task<int?> Execute(MessengerSource messengerSource)
    {
        messengerSource.Id = _sourceId;
        return await _messageSourceRepository.UpdateMessageSourceAsync(messengerSource, _transaction);
    }

    public override async Task<int?> Execute(PhoneSource phoneSource)
    {
        phoneSource.Id = _sourceId;
        return await _messageSourceRepository.UpdateMessageSourceAsync(phoneSource, _transaction);
    }
}