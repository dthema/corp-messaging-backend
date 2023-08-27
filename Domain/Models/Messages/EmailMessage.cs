using Dapper;
using Domain.Models.MessageSources;

namespace Domain.Models.Messages;

[Table("Messages")]
public class EmailMessage : Message
{
    public EmailMessage() => SourceType = SourceType.Email;
}