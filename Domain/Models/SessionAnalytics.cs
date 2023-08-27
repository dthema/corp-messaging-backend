using System.ComponentModel.DataAnnotations;
using Dapper;

namespace Domain.Models;

[Table("SessionsAnalytics")]
public class SessionAnalytics
{
    public int Id { get; set; }
    public int DepartmentId { get; set; }
    public int CheckedMessages { get; set; }
    [Dapper.Editable(true)] [DataType(DataType.Date)]
    public DateOnly SessionDate { get; set; }
}