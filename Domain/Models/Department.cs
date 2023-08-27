using Dapper;
using Domain.Models.Workers;

namespace Domain.Models;

[Table("Departments")]
public class Department
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<Employee> Employees { get; set; }
    public List<Manager> Managers { get; set; }
}