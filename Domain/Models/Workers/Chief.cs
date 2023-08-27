using Dapper;

namespace Domain.Models.Workers;

[Table("Workers")]
public class Chief : Worker
{
    public Chief() => Role = Role.Chief;
}