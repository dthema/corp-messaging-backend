using Application.Commands.Workers;
using Application.DTO;
using Application.Services.Interfaces;
using Application.Utils.WorkerDtoExecutors;
using Application.Utils.WorkerFactories;
using Domain.Exceptions;
using Domain.Models;
using Domain.Models.Workers;
using Domain.Repositories;

namespace Application.Services;

public class SessionService : ISessionService
{
    private readonly IRepositoriesManager _repositoriesManager;
    private readonly IWorkerFactory _workerFactory;

    public SessionService(IRepositoriesManager repositoriesManager)
    {
        ArgumentNullException.ThrowIfNull(repositoriesManager);
        _repositoriesManager = repositoriesManager;
        _workerFactory = new EmployeeFactory();
        _workerFactory
            .AddNext(new ManagerFactory())
            .AddNext(new ChiefFactory());
    }

    public async Task<SessionDto> Start(UnregisteredAccountDto accountDto)
    {
        using var transaction = await _repositoriesManager.SessionRepository.BeginTransactionAsync();

        Account account =
            await _repositoriesManager.AccountRepository.GetAccountByLoginAsync(accountDto.Login, transaction) ??
            throw new FieldNotFoundException();
        Session? session =
            await _repositoriesManager.SessionRepository.GetSessionByWorkerIdAsync(account.WorkerId, transaction);
        if (session is not null)
            throw new BadQueryException("Session already started");

        Worker worker = await _repositoriesManager.WorkerRepository.GetWorkerAsync(account.WorkerId, transaction) ??
                        throw new FieldNotFoundException();
        var getDepartmentCommand = new WorkerGetDepartmentIdCommand(_repositoriesManager.WorkerRepository,
            _repositoriesManager.DepartmentRepository, transaction);
        IWorkerDtoExecutor workerDtoExecutor = new RegisteredEmployeeDtoExecutor(getDepartmentCommand);
        workerDtoExecutor
            .AddNext(new RegisteredManagerDtoExecutor(getDepartmentCommand))
            .AddNext(new RegisteredChiefDtoExecutor(getDepartmentCommand));
        int departmentId = await workerDtoExecutor.DoWorkerDtoCommand(_workerFactory.CreateWorkerDto(worker)) ??
                           throw new FieldNotFoundException();

        var analyticsId = await _repositoriesManager.SessionAnalyticsRepository.InsertSessionAnalyticsAsync(
            new SessionAnalytics
            {
                DepartmentId = departmentId,
                SessionDate = DateOnly.FromDateTime(DateTime.Today)
            }, transaction) ?? throw new FieldNotFoundException();

        int newSessionId = await _repositoriesManager.SessionRepository.InsertSessionAsync(new Session
        {
            AnalyticsId = analyticsId,
            WorkerId = worker.Id
        }, transaction) ?? throw new FieldNotFoundException();

        transaction.Commit();
        transaction.Connection?.Close();

        return new SessionDto(newSessionId, worker.Id, analyticsId);
    }

    public async Task Finish(int sessionId)
    {
        using var transaction = await _repositoriesManager.SessionRepository.BeginTransactionAsync();

        Session session = await _repositoriesManager.SessionRepository.GetSessionAsync(sessionId, transaction) ??
                          throw new FieldNotFoundException();
        await _repositoriesManager.SessionRepository.DeleteSessionAsync(sessionId, transaction);

        transaction.Commit();
        transaction.Connection?.Close();
    }

    public async Task<SessionDto> GetById(int sessionId, int id)
    {
        using var transaction = await _repositoriesManager.SessionRepository.BeginTransactionAsync();

        var session = await _repositoriesManager.SessionRepository.GetSessionAsync(sessionId, transaction) ??
                      throw new FieldNotFoundException();
        var user = await _repositoriesManager.WorkerRepository.GetWorkerAsync(session.WorkerId, transaction) ??
                   throw new FieldNotFoundException();
        if (user.Role != Role.Chief)
            throw new FieldNotFoundException();

        Session requestSession = await _repositoriesManager.SessionRepository.GetSessionAsync(id, transaction) ??
                                 throw new FieldNotFoundException();

        transaction.Commit();
        transaction.Connection?.Close();

        return new SessionDto(requestSession.Id, requestSession.WorkerId, requestSession.AnalyticsId);
    }

    public async Task<SessionDto> GetByWorkerId(int sessionId, int workerId)
    {
        using var transaction = await _repositoriesManager.SessionRepository.BeginTransactionAsync();

        var session = await _repositoriesManager.SessionRepository.GetSessionAsync(sessionId, transaction) ??
                      throw new FieldNotFoundException();
        var user = await _repositoriesManager.WorkerRepository.GetWorkerAsync(session.WorkerId, transaction) ??
                   throw new FieldNotFoundException();
        if (user.Role != Role.Chief)
            throw new BadQueryException("Only cheafs can get worker by id");

        Session requestedSession =
            await _repositoriesManager.SessionRepository.GetSessionByWorkerIdAsync(workerId, transaction) ??
            throw new FieldNotFoundException();

        transaction.Commit();
        transaction.Connection?.Close();

        return new SessionDto(requestedSession.Id, requestedSession.WorkerId, requestedSession.AnalyticsId);
    }

    public async Task<IEnumerable<SessionDto>> GetAll(int sessionId)
    {
        using var transaction = await _repositoriesManager.SessionRepository.BeginTransactionAsync();

        var session = await _repositoriesManager.SessionRepository.GetSessionAsync(sessionId, transaction) ??
                      throw new FieldNotFoundException();
        var user = await _repositoriesManager.WorkerRepository.GetWorkerAsync(session.WorkerId, transaction) ??
                   throw new FieldNotFoundException();
        if (user.Role != Role.Chief)
            throw new BadQueryException("Only cheafs can get all workers");

        IEnumerable<Session> sessions = await _repositoriesManager.SessionRepository.GetAllSessionsAsync(transaction);

        transaction.Commit();
        transaction.Connection?.Close();

        return sessions.Select(session => new SessionDto(session.Id, session.WorkerId, session.AnalyticsId));
    }
}