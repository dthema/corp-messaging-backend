using Dapper;
using Domain.Models.MessageSources;
using Domain.Repositories;

namespace Domain.Models.Workers;

[Table("Workers")]
public abstract class Worker
{ 
    public int Id { get; set; }
    public string Firstname { get; set; }
    public string Lastname { get; set; }
    public Role Role { get; set; }
    public List<MessageSource> MessageSources { get; set; }
}