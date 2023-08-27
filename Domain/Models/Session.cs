using Dapper;

namespace Domain.Models;

[Table("Sessions")]
public class Session
{
    public int Id { get; set; }
    public int WorkerId { get; set; }
    public int AnalyticsId { get; set; }
}