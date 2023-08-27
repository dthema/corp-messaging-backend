using Dapper;

namespace Domain.Models;

[Table("WorkersSourcesMapping")]
public class WorkerSources
{
    [Key] [Required]
    public int WorkerId { get; set; }
    [Editable(true)]
    public int[] SourcesIds { get; set; }
}