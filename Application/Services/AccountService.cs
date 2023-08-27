using Application.DTO;
using Application.Services.Interfaces;
using Domain.Exceptions;
using Domain.Models;
using Domain.Repositories;

namespace Application.Services;

public class AccountService : IAccountService
{
    private readonly IRepositoriesManager _repositoriesManager;

    public AccountService(IRepositoriesManager repositoriesManager)
    {
        ArgumentNullException.ThrowIfNull(repositoriesManager);
        _repositoriesManager = repositoriesManager;
    }

    public async Task<AccountDto> Create(int sessionId, AccountDto accountDto)
    {
        EmptyStringBadQueryException.ThrowIfStringNull(accountDto.Login);
        EmptyStringBadQueryException.ThrowIfStringNull(accountDto.Password);
        
        using var transaction = await _repositoriesManager.AccountRepository.BeginTransactionAsync();

        var session = await _repositoriesManager.SessionRepository.GetSessionAsync(sessionId, transaction)
                      ?? throw new FieldNotFoundException();
        var user = await _repositoriesManager.WorkerRepository.GetWorkerAsync(session.WorkerId, transaction)
                   ?? throw new FieldNotFoundException();
        if (user.Role == Role.Employee)
            throw new BadQueryException("Employee cannot create account");

        var worker = _repositoriesManager.WorkerRepository.GetWorkerAsync(accountDto.WorkerId, transaction)
                     ?? throw new FieldNotFoundException();
        var existedAccount = await _repositoriesManager.AccountRepository.GetAccountByLoginAsync(accountDto.Login, transaction);
        if (existedAccount is not null)
            throw new BadQueryException("Account with this login already exists");
        existedAccount = await _repositoriesManager.AccountRepository.GetAccountByWorkerIdAsync(accountDto.WorkerId, transaction);
        if (existedAccount is not null)
            throw new BadQueryException("Account for this worker already exists");

        int id = await _repositoriesManager.AccountRepository.InsertAccountAsync(new Account
        {
            Login = accountDto.Login,
            Password = accountDto.Password,
            WorkerId = accountDto.WorkerId
        }, transaction) ?? throw new FieldNotFoundException();
        var createdAccount = await _repositoriesManager.AccountRepository.GetAccountByWorkerIdAsync(id, transaction)
                             ?? throw new FieldNotFoundException();

        transaction.Commit();
        transaction.Connection?.Close();

        return new AccountDto(createdAccount.Login, createdAccount.Password, createdAccount.WorkerId);
    }

    public async Task<AccountDto> Update(int sessionId, AccountDto accountDto)
    {
        EmptyStringBadQueryException.ThrowIfStringNull(accountDto.Login);
        EmptyStringBadQueryException.ThrowIfStringNull(accountDto.Password);

        using var transaction = await _repositoriesManager.AccountRepository.BeginTransactionAsync();

        var session = await _repositoriesManager.SessionRepository.GetSessionAsync(sessionId, transaction)
                      ?? throw new FieldNotFoundException();
        var user = await _repositoriesManager.WorkerRepository.GetWorkerAsync(session.WorkerId, transaction)
                   ?? throw new FieldNotFoundException();
        if (user.Role == Role.Employee)
            throw new BadQueryException("Employee cannot update account");

        await _repositoriesManager.AccountRepository.UpdateAccountAsync(new Account
        {
            Login = accountDto.Login,
            Password = accountDto.Password,
            WorkerId = accountDto.WorkerId
        }, transaction);
        var updatedAccount =
            await _repositoriesManager.AccountRepository.GetAccountByWorkerIdAsync(accountDto.WorkerId, transaction)
            ?? throw new FieldNotFoundException();

        transaction.Commit();
        transaction.Connection?.Close();

        return new AccountDto(updatedAccount.Login, updatedAccount.Password, updatedAccount.WorkerId);
    }

    public async Task Delete(int sessionId, int workerId)
    {
        using var transaction = await _repositoriesManager.AccountRepository.BeginTransactionAsync();

        var session = await _repositoriesManager.SessionRepository.GetSessionAsync(sessionId, transaction)
                      ?? throw new FieldNotFoundException();
        var user = await _repositoriesManager.WorkerRepository.GetWorkerAsync(session.WorkerId, transaction)
                   ?? throw new FieldNotFoundException();
        if (user.Role == Role.Employee)
            throw new BadQueryException("Employee cannot delete account");

        Account account = await _repositoriesManager.AccountRepository.GetAccountByWorkerIdAsync(workerId, transaction)
                          ?? throw new FieldNotFoundException();
        await _repositoriesManager.AccountRepository.DeleteAccountAsync(workerId, transaction);

        transaction.Commit();
        transaction.Connection?.Close();
    }

    public async Task<AccountDto> GetByWorkerId(int sessionId, int workerId)
    {
        using var transaction = await _repositoriesManager.AccountRepository.BeginTransactionAsync();

        var session = await _repositoriesManager.SessionRepository.GetSessionAsync(sessionId, transaction)
                      ?? throw new FieldNotFoundException();
        var user = await _repositoriesManager.WorkerRepository.GetWorkerAsync(session.WorkerId, transaction)
                   ?? throw new FieldNotFoundException();
        if (user.Role == Role.Employee && user.Id != workerId)
            throw new BadQueryException("Employee cannot get someone else's account");

        var account = await _repositoriesManager.AccountRepository.GetAccountByWorkerIdAsync(workerId, transaction)
                      ?? throw new FieldNotFoundException();

        transaction.Commit();
        transaction.Connection?.Close();

        return new AccountDto(account.Login, account.Password, account.WorkerId);
    }

    public async Task<AccountDto> GetByLogin(int sessionId, string login)
    {
        EmptyStringBadQueryException.ThrowIfStringNull(login);

        using var transaction = await _repositoriesManager.AccountRepository.BeginTransactionAsync();

        var session = await _repositoriesManager.SessionRepository.GetSessionAsync(sessionId, transaction)
                      ?? throw new FieldNotFoundException();
        var user = await _repositoriesManager.WorkerRepository.GetWorkerAsync(session.WorkerId, transaction)
                   ?? throw new FieldNotFoundException();
        var account = await _repositoriesManager.AccountRepository.GetAccountByLoginAsync(login, transaction)
                      ?? throw new FieldNotFoundException();
        if (user.Role == Role.Employee && user.Id != account.WorkerId)
            throw new BadQueryException("Employee cannot get someone else's account");

        transaction.Commit();
        transaction.Connection?.Close();

        return new AccountDto(account.Login, account.Password, account.WorkerId);
    }

    public async Task<IEnumerable<AccountDto>> GetAll(int sessionId)
    {
        using var transaction = await _repositoriesManager.AccountRepository.BeginTransactionAsync();

        var session = await _repositoriesManager.SessionRepository.GetSessionAsync(sessionId, transaction)
                      ?? throw new FieldNotFoundException();
        var user = await _repositoriesManager.WorkerRepository.GetWorkerAsync(session.WorkerId, transaction)
                   ?? throw new FieldNotFoundException();
        if (user.Role == Role.Employee)
            throw new BadQueryException("Employee cannot get someone else's account");

        var accounts = await _repositoriesManager.AccountRepository.GetAllAccountsAsync(transaction);

        transaction.Commit();
        transaction.Connection?.Close();

        return accounts.Select(account => new AccountDto(account.Login, account.Password, account.WorkerId));
    }
}