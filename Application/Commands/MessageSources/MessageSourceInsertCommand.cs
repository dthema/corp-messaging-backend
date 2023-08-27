using System.Data;
using Domain.Models.MessageSources;
using Domain.Repositories;

namespace Application.Commands.MessageSources;

public class MessageSourceInsertCommand : MessageSourceCommand
{
    public MessageSourceInsertCommand(IMessageSourceRepository messageSourceRepository, IDbTransaction? transaction = null) 
        : base(messageSourceRepository, transaction) { }

    public override async Task<int?> Execute(EmailSource emailSource)
    {
        return await _messageSourceRepository.InsertMessageSourceAsync(emailSource, _transaction);
    }

    public override async Task<int?> Execute(MessengerSource messengerSource)
    {
        return await _messageSourceRepository.InsertMessageSourceAsync(messengerSource, _transaction);
    }

    public override async Task<int?> Execute(PhoneSource phoneSource)
    {
        return await _messageSourceRepository.InsertMessageSourceAsync(phoneSource, _transaction);
    }
}