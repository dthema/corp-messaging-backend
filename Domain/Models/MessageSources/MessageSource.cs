using Dapper;
using Domain.Models.Messages;

namespace Domain.Models.MessageSources;

[Table("Sources")]
public abstract class MessageSource
{
    public int Id { get; set; }
    public SourceType SourceType { get; set; }
    public List<Message> OutgoingMessages { get; set; }
    public List<Message> IncomingMessages { get; set; }
}