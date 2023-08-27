using Dapper;
using Domain.Models.MessageSources;

namespace Domain.Models.Messages;

[Table("Messages")]
public class PhoneMessage : Message
{
    public PhoneMessage() => SourceType = MessageSources.SourceType.Phone;
}