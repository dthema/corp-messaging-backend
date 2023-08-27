using System.Data;
using Dapper;
using Domain.Models;
using Domain.Repositories;
using Npgsql;

namespace Infrastructure.Repositories;

public class AccountRepository : BaseRepository, IAccountRepository
{
    public AccountRepository()
    {
        SimpleCRUD.SetDialect(SimpleCRUD.Dialect.PostgreSQL);
    }
    
    public async Task<int?> InsertAccountAsync(Account account, IDbTransaction? transaction = null) => await InsertAsync(account, transaction);

    public async Task<int> DeleteAccountAsync(int workerId, IDbTransaction? transaction = null) => await DeleteAsync<Account>(workerId, transaction);

    public async Task DeleteAllAccountsAsync(IDbTransaction? transaction = null) => await DeleteAllAsync<Account>(transaction);

    public async Task<int> UpdateAccountAsync(Account account, IDbTransaction? transaction = null) => await UpdateAsync(account, transaction);

    public async Task<Account> GetAccountByWorkerIdAsync(int workerId, IDbTransaction? transaction = null) => await GetAsync<Account>(workerId, transaction);

    public async Task<Account?> GetAccountByLoginAsync(string login, IDbTransaction? transaction = null)
    {
        await using var connection = new NpgsqlConnection(ConnectionString);
        var accounts = await connection.GetListAsync<Account>(
            "WHERE \"Login\" = @Login",
            new { Login = login }, transaction);
        return accounts.FirstOrDefault(x => x.Login == login);
    }

    public async Task<IEnumerable<Account>> GetAllAccountsAsync(IDbTransaction? transaction = null) => await GetListAsync<Account>(transaction);
}