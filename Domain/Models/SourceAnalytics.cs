using System.ComponentModel.DataAnnotations;
using Dapper;

namespace Domain.Models;

[Table("SourcesAnalytics")]
public class SourceAnalytics
{
    public int Id { get; set; }
    public int MessageSourceId { get; set; }
    public int MessagesCount { get; set; }
    [Dapper.Editable(true)] [DataType(DataType.Date)]
    public DateOnly SessionDate { get; set; }
}