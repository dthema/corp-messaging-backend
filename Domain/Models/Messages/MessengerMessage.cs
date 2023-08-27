using Dapper;
using Domain.Models.MessageSources;

namespace Domain.Models.Messages;

[Table("Messages")]
public class MessengerMessage : Message
{
    public MessengerMessage() => SourceType = MessageSources.SourceType.Messenger;
}