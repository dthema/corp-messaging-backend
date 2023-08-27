using Dapper;
namespace Domain.Models.MessageSources;

[Table("Sources")]
public class EmailSource : MessageSource
{
    public EmailSource() => SourceType = SourceType.Email;
    
    public string Email { get; set; }
}