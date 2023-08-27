using Dapper;

namespace Domain.Models.MessageSources;

[Table("Sources")]
public class MessengerSource : MessageSource
{
    public MessengerSource() => SourceType = SourceType.Messenger;
    
    public string MessengerName { get; set; }
    public string Username { get; set; }
}