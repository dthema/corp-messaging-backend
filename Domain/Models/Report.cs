using System.ComponentModel.DataAnnotations;
using Dapper;

namespace Domain.Models;

[Table("Reports")]
public class Report
{
    public int Id { get; set; }
    public int DepartmentId { get; set; }
    public int CheckedMessages { get; set; }
    public int MessagesByDate { get; set; }
    [Dapper.Editable(true)] [DataType(DataType.Date)]
    public DateOnly Date { get; set; }
    public List<SourceAnalytics> SourcesAnalytics { get; set; }
}