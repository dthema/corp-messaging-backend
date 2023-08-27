using Dapper;

namespace Domain.Models.Workers;

[Table("Workers")]
public class Employee : Worker, IDepartmentalWorker
{
    public Employee() => Role = Role.Employee;
    
    public int DepartmentId { get; set; }
}