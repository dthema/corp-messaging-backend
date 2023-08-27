using Dapper;

namespace Domain.Models;

[Table("Accounts")]
public class Account
{

    [Key] [Required]
    public int WorkerId { get; set; }
    public string Login { get; set; }
    public string Password { get; set; }
}