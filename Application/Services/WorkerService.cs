using Application.Commands.Workers;
using Application.DTO.UnregisteredWorkers;
using Application.DTO.Workers;
using Application.Services.Interfaces;
using Application.Utils.WorkerDtoExecutors;
using Application.Utils.WorkerFactories;
using Domain.Exceptions;
using Domain.Models;
using Domain.Models.Workers;
using Domain.Repositories;

namespace Application.Services;

public class WorkerService : IWorkerService
{
    private readonly IRepositoriesManager _repositoriesManager;
    private readonly IWorkerFactory _workerFactory;

    public WorkerService(IRepositoriesManager repositoriesManager)
    {
        ArgumentNullException.ThrowIfNull(repositoriesManager);
        _repositoriesManager = repositoriesManager;
        _workerFactory = new EmployeeFactory();
        _workerFactory
            .AddNext(new ManagerFactory())
            .AddNext(new ChiefFactory());
    }

    public async Task<WorkerDto> Create(int sessionId, UnregisteredWorkerDto workerDto)
    {
        using var transaction = await _repositoriesManager.WorkerRepository.BeginTransactionAsync();

        var session = await _repositoriesManager.SessionRepository.GetSessionAsync(sessionId, transaction) ??
                      throw new FieldNotFoundException();
        var user = await _repositoriesManager.WorkerRepository.GetWorkerAsync(session.WorkerId, transaction) ??
                   throw new FieldNotFoundException();
        if (user.Role == Role.Employee)
            throw new BadQueryException("Employee cannot manage workers");
        if (workerDto.Role == Role.Chief)
            throw new BadQueryException("Chiefs created from Initialization Service");
        if (workerDto.Role == user.Role)
            throw new BadQueryException("Manager cannot create another manager");

        var insertCommand = new WorkerInsertCommand(_repositoriesManager.WorkerRepository,
            _repositoriesManager.DepartmentRepository, transaction);
        IWorkerDtoExecutor workerDtoExecutor = new UnregisteredEmployeeDtoExecutor(insertCommand);
        workerDtoExecutor
            .AddNext(new UnregisteredManagerDtoExecutor(insertCommand))
            .AddNext(new UnregisteredChiefDtoExecutor(insertCommand));
        int workerId = await workerDtoExecutor.DoWorkerDtoCommand(workerDto) ?? throw new FieldNotFoundException();

        Worker worker = await _repositoriesManager.WorkerRepository.GetWorkerAsync(workerId, transaction) ??
                        throw new FieldNotFoundException();

        transaction.Commit();
        transaction.Connection?.Close();

        return _workerFactory.CreateWorkerDto(worker);
    }

    public async Task<WorkerDto> Update(int sessionId, int workerId, UnregisteredWorkerDto workerDto)
    {
        using var transaction = await _repositoriesManager.WorkerRepository.BeginTransactionAsync();

        var session = await _repositoriesManager.SessionRepository.GetSessionAsync(sessionId, transaction) ??
                      throw new FieldNotFoundException();
        var user = await _repositoriesManager.WorkerRepository.GetWorkerAsync(session.WorkerId, transaction) ??
                   throw new FieldNotFoundException();
        if (user.Role == Role.Employee)
            throw new BadQueryException("Employee cannot manage workers");
        if (user.Role == Role.Manager && workerDto.Role != Role.Employee)
            throw new BadQueryException("Manager cannot update another manager or chief");

        var updateCommand = new WorkerUpdateCommand(_repositoriesManager.WorkerRepository,
            _repositoriesManager.DepartmentRepository, workerId, transaction);
        IWorkerDtoExecutor workerDtoExecutor = new UnregisteredEmployeeDtoExecutor(updateCommand);
        workerDtoExecutor
            .AddNext(new UnregisteredManagerDtoExecutor(updateCommand))
            .AddNext(new UnregisteredChiefDtoExecutor(updateCommand));
        await workerDtoExecutor.DoWorkerDtoCommand(workerDto);

        Worker worker = await _repositoriesManager.WorkerRepository.GetWorkerAsync(workerId) ??
                        throw new FieldNotFoundException();

        transaction.Commit();
        transaction.Connection?.Close();

        return _workerFactory.CreateWorkerDto(worker);
    }

    public async Task Delete(int sessionId, int workerId)
    {
        using var transaction = await _repositoriesManager.WorkerRepository.BeginTransactionAsync();

        var session = await _repositoriesManager.SessionRepository.GetSessionAsync(sessionId, transaction) ??
                      throw new FieldNotFoundException();
        var user = await _repositoriesManager.WorkerRepository.GetWorkerAsync(session.WorkerId, transaction) ??
                   throw new FieldNotFoundException();
        Worker worker = await _repositoriesManager.WorkerRepository.GetWorkerAsync(workerId, transaction) ??
                        throw new FieldNotFoundException();
        if (user.Role == Role.Employee)
            throw new BadQueryException("Employee cannot manage workers");
        if (user.Role == Role.Manager && worker.Role != Role.Employee)
            throw new BadQueryException("Manager cannot delete another manager or chief");

        var workerSource = await _repositoriesManager.WorkerSourcesRepository.GetWorkerSourcesByWorkerIdAsync(workerId, transaction);
        
        foreach (int sourceId in workerSource?.SourcesIds ?? Array.Empty<int>())
            await _repositoriesManager.MessageSourceRepository.DeleteMessageSourceAsync(sourceId, transaction);

        if (workerSource is not null)
            await _repositoriesManager.WorkerSourcesRepository.DeleteWorkerSourcesAsync(workerSource.WorkerId, transaction);

        var workerSession = await _repositoriesManager.SessionRepository.GetSessionByWorkerIdAsync(workerId, transaction);
        if (workerSession is not null)
            await _repositoriesManager.SessionRepository.DeleteSessionAsync(workerSession.Id);
        
        await _repositoriesManager.AccountRepository.DeleteAccountAsync(workerId, transaction); 
        await _repositoriesManager.WorkerRepository.DeleteWorkerAsync(workerId, transaction);
        
        transaction.Commit();
        transaction.Connection?.Close();
    }

    public async Task<WorkerDto> GetById(int sessionId, int workerId)
    {
        using var transaction = await _repositoriesManager.WorkerRepository.BeginTransactionAsync();

        var session = await _repositoriesManager.SessionRepository.GetSessionAsync(sessionId, transaction) ??
                      throw new FieldNotFoundException();
        var user = await _repositoriesManager.WorkerRepository.GetWorkerAsync(session.WorkerId, transaction) ??
                   throw new FieldNotFoundException();
        if (user.Role == Role.Employee)
            throw new BadQueryException("Employee cannot manage workers");

        Worker worker = await _repositoriesManager.WorkerRepository.GetWorkerAsync(workerId, transaction) ??
                        throw new FieldNotFoundException();

        transaction.Commit();
        transaction.Connection?.Close();

        return _workerFactory.CreateWorkerDto(worker);
    }

    public async Task<IEnumerable<WorkerDto>> GetByDepartmentId(int sessionId, int departmentId)
    {
        using var transaction = await _repositoriesManager.WorkerRepository.BeginTransactionAsync();

        var session = await _repositoriesManager.SessionRepository.GetSessionAsync(sessionId, transaction) ??
                      throw new FieldNotFoundException();
        var user = await _repositoriesManager.WorkerRepository.GetWorkerAsync(session.WorkerId, transaction) ??
                   throw new FieldNotFoundException();
        if (user.Role == Role.Employee)
            throw new BadQueryException("Employee cannot manage workers");

        var workers =
            await _repositoriesManager.WorkerRepository.GetWorkersByDepartmentIdAsync(departmentId, transaction);

        transaction.Commit();
        transaction.Connection?.Close();

        return workers.Select(worker => _workerFactory.CreateWorkerDto(worker));
    }

    public async Task<IEnumerable<WorkerDto>> GetByRole(int sessionId, Role role)
    {
        using var transaction = await _repositoriesManager.WorkerRepository.BeginTransactionAsync();

        var session = await _repositoriesManager.SessionRepository.GetSessionAsync(sessionId, transaction) ??
                      throw new FieldNotFoundException();
        var user = await _repositoriesManager.WorkerRepository.GetWorkerAsync(session.WorkerId, transaction) ??
                   throw new FieldNotFoundException();
        if (user.Role == Role.Employee)
            throw new BadQueryException("Employee cannot manage workers");

        var workers = await _repositoriesManager.WorkerRepository.GetWorkersByRoleAsync(role, transaction);

        transaction.Commit();
        transaction.Connection?.Close();

        return workers.Select(worker => _workerFactory.CreateWorkerDto(worker));
    }

    public async Task<IEnumerable<WorkerDto>> GetAll(int sessionId)
    {
        using var transaction = await _repositoriesManager.WorkerRepository.BeginTransactionAsync();

        var session = await _repositoriesManager.SessionRepository.GetSessionAsync(sessionId, transaction) ??
                      throw new FieldNotFoundException();
        var user = await _repositoriesManager.WorkerRepository.GetWorkerAsync(session.WorkerId, transaction) ??
                   throw new FieldNotFoundException();
        if (user.Role == Role.Employee)
            throw new BadQueryException("Employee cannot manage workers");

        var workers = await _repositoriesManager.WorkerRepository.GetAllWorkersAsync(transaction);

        transaction.Commit();
        transaction.Connection?.Close();

        return workers.Select(worker => _workerFactory.CreateWorkerDto(worker));
    }
}