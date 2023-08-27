using Application.DTO;
using Application.Services.Interfaces;
using Domain.Exceptions;
using Domain.Models;
using Domain.Models.Workers;
using Domain.Repositories;

namespace Application.Services;

public class InitializationService : IInitializationService
{
    private readonly IRepositoriesManager _repositoriesManager;

    public InitializationService(IRepositoriesManager repositoriesManager)
    {
        ArgumentNullException.ThrowIfNull(repositoriesManager);
        _repositoriesManager = repositoriesManager;
    }

    public async Task<ChiefAccountDto> CreateChiefWithAccount(UnregisteredChiefAccountDto chiefAccountDto)
    {
        using var transaction = await _repositoriesManager.DepartmentRepository.BeginTransactionAsync();

        int chiefId = await _repositoriesManager.WorkerRepository.InsertWorkerAsync(new Chief
        {
            Firstname = chiefAccountDto.Firstname,
            Lastname = chiefAccountDto.Lastname
        }, transaction) ?? throw new FieldNotFoundException();

        await _repositoriesManager.AccountRepository.InsertAccountAsync(new Account
        {
            Login = chiefAccountDto.Login,
            Password = chiefAccountDto.Password,
            WorkerId = chiefId
        }, transaction);

        Account account =
            await _repositoriesManager.AccountRepository.GetAccountByWorkerIdAsync(chiefId, transaction) ??
            throw new FieldNotFoundException();

        transaction.Commit();
        transaction.Connection?.Close();

        return new ChiefAccountDto(chiefId, account.Login);
    }
}