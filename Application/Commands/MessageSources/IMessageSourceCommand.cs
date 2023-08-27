using Domain.Models.MessageSources;
using Domain.Models.Workers;

namespace Application.Commands.MessageSources;

public interface IMessageSourceCommand
{
    Task<int?> Execute(EmailSource emailSource);
    Task<int?> Execute(MessengerSource messengerSource);
    Task<int?> Execute(PhoneSource phoneSource);
}