using Dapper;
using Domain.Models.MessageSources;

namespace Domain.Models.Messages;

[Table("Messages")]
public abstract class Message
{
    public int Id { get; set; }
    public string Text { get; set; }
    public int SenderSourceId { get; set; }
    public int RecipientSourceId { get; set; }
    public bool Checked { get; set; }
    public SourceType SourceType { get; set; }
    public DateTime Date { get; set; }
}