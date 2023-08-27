using System.Data;
using Domain.Models.MessageSources;
using Domain.Repositories;

namespace Application.Commands.MessageSources;

public abstract class MessageSourceCommand : IMessageSourceCommand
{
    protected readonly IMessageSourceRepository _messageSourceRepository;
    protected IDbTransaction? _transaction;

    public MessageSourceCommand(IMessageSourceRepository messageSourceRepository, IDbTransaction? transaction = null)
    {
        _messageSourceRepository = messageSourceRepository;
        _transaction = transaction;
    }

    public abstract Task<int?> Execute(EmailSource emailSource);
    public abstract Task<int?> Execute(MessengerSource messengerSource);
    public abstract Task<int?> Execute(PhoneSource phoneSource);
}