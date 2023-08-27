using Dapper;

namespace Domain.Models.MessageSources;

[Table("Sources")]
public class PhoneSource : MessageSource
{
    public PhoneSource() => SourceType = SourceType.Phone;
    
    public string PhoneNumber { get; set; }
}