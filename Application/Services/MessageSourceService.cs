using Application.Commands.MessageSources;
using Application.DTO.MessageSources;
using Application.DTO.UnregisteredMessageSources;
using Application.Services.Interfaces;
using Application.Utils.MessageSourceDtoExecutors;
using Application.Utils.MessageSourceFactories;
using Domain.Exceptions;
using Domain.Models;
using Domain.Models.MessageSources;
using Domain.Models.Workers;
using Domain.Repositories;

namespace Application.Services;

public class MessageSourceService : IMessageSourceService
{
    private readonly IRepositoriesManager _repositoriesManager;
    private readonly IMessageSourceFactory _messageSourceFactory;

    public MessageSourceService(IRepositoriesManager repositoriesManager)
    {
        ArgumentNullException.ThrowIfNull(repositoriesManager);
        _repositoriesManager = repositoriesManager;
        _messageSourceFactory = new EmailSourceFactory();
        _messageSourceFactory
            .AddNext(new MessengerSourceFactory())
            .AddNext(new PhoneSourceFactory());
    }

    public async Task<MessageSourceDto> CreateExternal(UnregisteredMessageSourceDto unregisteredMessageSourceDto)
    {
        using var transaction = await _repositoriesManager.MessageSourceRepository.BeginTransactionAsync();

        var insertCommand = new MessageSourceInsertCommand(_repositoriesManager.MessageSourceRepository, transaction);
        IMessageSourceDtoExecutor messageSourceDtoExecutor = new UnregisteredEmailSourceDtoExecutor(insertCommand);
        messageSourceDtoExecutor
            .AddNext(new UnregisteredMessengerSourceDtoExecutor(insertCommand))
            .AddNext(new UnregisteredPhoneSourceDtoExecutor(insertCommand));
        int messageSourceId = await messageSourceDtoExecutor.DoMessageSourceDtoCommand(unregisteredMessageSourceDto) ??
                              throw new FieldNotFoundException();
        var messageSource =
            await _repositoriesManager.MessageSourceRepository.GetMessageSourceAsync(messageSourceId, transaction) ??
            throw new FieldNotFoundException();

        transaction.Commit();
        transaction.Connection?.Close();

        return _messageSourceFactory.CreateMessageSourceDto(messageSource);
    }

    public async Task<MessageSourceDto> AddForWorker(int sessionId, int workerId,
        UnregisteredMessageSourceDto unregisteredMessageSourceDto)
    {
        using var transaction = await _repositoriesManager.MessageSourceRepository.BeginTransactionAsync();

        var session = await _repositoriesManager.SessionRepository.GetSessionAsync(sessionId, transaction) ??
                      throw new FieldNotFoundException();
        var user = await _repositoriesManager.WorkerRepository.GetWorkerAsync(session.WorkerId, transaction) ??
                   throw new FieldNotFoundException();
        Worker worker = await _repositoriesManager.WorkerRepository.GetWorkerAsync(workerId, transaction) ??
                        throw new FieldNotFoundException();
        if (user.Role == Role.Employee)
            throw new BadQueryException("Employee cannot manage sources");
        if (worker.Role == Role.Chief && user.Role == Role.Manager)
            throw new BadQueryException("Manager cannot manage chiefs sources");

        var insertCommand = new MessageSourceInsertCommand(_repositoriesManager.MessageSourceRepository, transaction);
        IMessageSourceDtoExecutor messageSourceDtoExecutor = new UnregisteredEmailSourceDtoExecutor(insertCommand);
        messageSourceDtoExecutor
            .AddNext(new UnregisteredMessengerSourceDtoExecutor(insertCommand))
            .AddNext(new UnregisteredPhoneSourceDtoExecutor(insertCommand));
        int messageSourceId = await messageSourceDtoExecutor.DoMessageSourceDtoCommand(unregisteredMessageSourceDto) ??
                              throw new FieldNotFoundException();

        WorkerSources? workerSources =
            await _repositoriesManager.WorkerSourcesRepository.GetWorkerSourcesByWorkerIdAsync(workerId, transaction);
        if (workerSources is null)
        {
            await _repositoriesManager.WorkerSourcesRepository.InsertWorkerSourcesAsync(new WorkerSources
            {
                WorkerId = workerId,
                SourcesIds = new[] { messageSourceId }
            }, transaction);
        }
        else
        {
            var messageSourcesIds = new List<int>(workerSources.SourcesIds) { messageSourceId };
            workerSources.SourcesIds = messageSourcesIds.ToArray();
            await _repositoriesManager.WorkerSourcesRepository.UpdateWorkerSourcesAsync(workerSources, transaction);
        }

        var messageSource =
            await _repositoriesManager.MessageSourceRepository.GetMessageSourceAsync(messageSourceId, transaction) ??
            throw new FieldNotFoundException();

        transaction.Commit();
        transaction.Connection?.Close();

        return _messageSourceFactory.CreateMessageSourceDto(messageSource);
    }

    public async Task<MessageSourceDto> Update(int sessionId, int workerId, int messageSourceId,
        UnregisteredMessageSourceDto messageSourceDto)
    {
        using var transaction = await _repositoriesManager.MessageSourceRepository.BeginTransactionAsync();

        Session session = await _repositoriesManager.SessionRepository.GetSessionAsync(sessionId, transaction) ??
                          throw new FieldNotFoundException();
        Worker user = await _repositoriesManager.WorkerRepository.GetWorkerAsync(session.WorkerId, transaction) ??
                      throw new FieldNotFoundException();
        Worker worker = await _repositoriesManager.WorkerRepository.GetWorkerAsync(workerId, transaction) ??
                        throw new FieldNotFoundException();
        if (user.Role == Role.Employee)
            throw new BadQueryException("Employee cannot manage sources");
        if (worker.Role == Role.Chief && user.Role == Role.Manager)
            throw new BadQueryException("Manager cannot manage chiefs sources");
        if (worker.MessageSources.All(x => x.Id != messageSourceId))
            throw new FieldNotFoundException();

        var updateCommand = new MessageSourceUpdateCommand(_repositoriesManager.MessageSourceRepository,
            messageSourceId, transaction);
        IMessageSourceDtoExecutor messageSourceDtoExecutor = new UnregisteredEmailSourceDtoExecutor(updateCommand);
        messageSourceDtoExecutor
            .AddNext(new UnregisteredMessengerSourceDtoExecutor(updateCommand))
            .AddNext(new UnregisteredPhoneSourceDtoExecutor(updateCommand));
        await messageSourceDtoExecutor.DoMessageSourceDtoCommand(messageSourceDto);

        MessageSource updatedMessageSource =
            await _repositoriesManager.MessageSourceRepository.GetMessageSourceAsync(messageSourceId, transaction) ??
            throw new FieldNotFoundException();

        transaction.Commit();
        transaction.Connection?.Close();

        return _messageSourceFactory.CreateMessageSourceDto(updatedMessageSource);
    }

    public async Task DeleteForWorker(int sessionId, int workerId, int messageSourceId)
    {
        using var transaction = await _repositoriesManager.MessageSourceRepository.BeginTransactionAsync();

        Session session = await _repositoriesManager.SessionRepository.GetSessionAsync(sessionId, transaction) ??
                          throw new FieldNotFoundException();
        Worker user = await _repositoriesManager.WorkerRepository.GetWorkerAsync(session.WorkerId, transaction) ??
                      throw new FieldNotFoundException();
        Worker worker = await _repositoriesManager.WorkerRepository.GetWorkerAsync(workerId, transaction) ??
                        throw new FieldNotFoundException();
        if (user.Role == Role.Employee)
            throw new BadQueryException("Employee cannot manage sources");
        if (worker.Role == Role.Chief && user.Role == Role.Manager)
            throw new BadQueryException("Manager cannot manage chiefs sources");
        if (worker.MessageSources.All(x => x.Id != messageSourceId))
            throw new FieldNotFoundException();

        await _repositoriesManager.MessageSourceRepository.DeleteMessageSourceAsync(messageSourceId, transaction);

        WorkerSources workerSource =
            await _repositoriesManager.WorkerSourcesRepository.GetWorkerSourcesByWorkerIdAsync(workerId, transaction) ??
            throw new FieldNotFoundException();
        var messageSourcesIds = new List<int>(workerSource.SourcesIds);
        messageSourcesIds.Remove(messageSourceId);
        workerSource.SourcesIds = messageSourcesIds.ToArray();
        await _repositoriesManager.WorkerSourcesRepository.UpdateWorkerSourcesAsync(workerSource, transaction);

        transaction.Commit();
        transaction.Connection?.Close();
    }

    public async Task<MessageSourceDto> GetById(int sessionId, int sourceId)
    {
        using var transaction = await _repositoriesManager.MessageSourceRepository.BeginTransactionAsync();

        Session session = await _repositoriesManager.SessionRepository.GetSessionAsync(sessionId, transaction) ??
                          throw new FieldNotFoundException();
        Worker user = await _repositoriesManager.WorkerRepository.GetWorkerAsync(session.WorkerId, transaction) ??
                      throw new FieldNotFoundException();
        if (user.Role == Role.Employee)
            throw new BadQueryException("Employee cannot manage sources");

        MessageSource messageSource =
            await _repositoriesManager.MessageSourceRepository.GetMessageSourceAsync(sourceId, transaction) ??
            throw new FieldNotFoundException();

        transaction.Commit();
        transaction.Connection?.Close();

        return _messageSourceFactory.CreateMessageSourceDto(messageSource);
    }

    public async Task<IEnumerable<MessageSourceDto>> GetByWorkerId(int sessionId, int workerId)
    {
        using var transaction = await _repositoriesManager.MessageSourceRepository.BeginTransactionAsync();

        Session session = await _repositoriesManager.SessionRepository.GetSessionAsync(sessionId, transaction) ??
                          throw new FieldNotFoundException();
        Worker user = await _repositoriesManager.WorkerRepository.GetWorkerAsync(session.WorkerId, transaction) ??
                      throw new FieldNotFoundException();
        Worker worker = await _repositoriesManager.WorkerRepository.GetWorkerAsync(workerId, transaction) ??
                        throw new FieldNotFoundException();
        if (user.Role == Role.Employee)
            throw new BadQueryException("Employee cannot manage sources");
        if (worker.Role == Role.Chief && user.Role == Role.Manager)
            throw new BadQueryException("Manager cannot manage chiefs sources");

        var messageSources =
            await _repositoriesManager.MessageSourceRepository.GetMessageSourcesByWorkerIdAsync(workerId, transaction);

        transaction.Commit();
        transaction.Connection?.Close();

        return messageSources.Select(messageSource => _messageSourceFactory.CreateMessageSourceDto(messageSource));
    }

    public async Task<IEnumerable<MessageSourceDto>> GetAll(int sessionId)
    {
        using var transaction = await _repositoriesManager.MessageSourceRepository.BeginTransactionAsync();

        Session session = await _repositoriesManager.SessionRepository.GetSessionAsync(sessionId, transaction) ??
                          throw new FieldNotFoundException();
        Worker user = await _repositoriesManager.WorkerRepository.GetWorkerAsync(session.WorkerId, transaction) ??
                      throw new FieldNotFoundException();
        if (user.Role != Role.Chief)
            throw new BadQueryException("Only chiefs can get all sources");

        var messageSources = await _repositoriesManager.MessageSourceRepository.GetAllMessageSourcesAsync(transaction);

        transaction.Commit();
        transaction.Connection?.Close();

        return messageSources.Select(messageSource => _messageSourceFactory.CreateMessageSourceDto(messageSource));
    }
}