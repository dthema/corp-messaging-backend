using System.Data;
using Domain.Models;

namespace Domain.Repositories;

public interface IAccountRepository : IBaseRepository
{
    Task<int?> InsertAccountAsync(Account account, IDbTransaction? transaction = null);
    Task<int> DeleteAccountAsync(int workerId, IDbTransaction? transaction = null);
    Task DeleteAllAccountsAsync(IDbTransaction? transaction = null);
    Task<int> UpdateAccountAsync(Account account, IDbTransaction? transaction = null); 
    Task<Account> GetAccountByWorkerIdAsync(int workerId, IDbTransaction? transaction = null);
    Task<Account?> GetAccountByLoginAsync(string login, IDbTransaction? transaction = null);
    Task<IEnumerable<Account>> GetAllAccountsAsync(IDbTransaction? transaction = null);
}