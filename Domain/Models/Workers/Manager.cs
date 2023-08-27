using Dapper;

namespace Domain.Models.Workers;

[Table("Workers")]
public class Manager : Worker, IDepartmentalWorker
{
    public Manager() => Role = Role.Manager;
    
    public int DepartmentId { get; set; }
}