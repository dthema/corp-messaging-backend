using Application.DTO.Messages;
using Application.Services.Interfaces;
using Application.Utils.MessageFactories;
using Domain.Exceptions;
using Domain.Models;
using Domain.Repositories;

namespace Application.Services;

public class MessageService : IMessageService
{
    private readonly IRepositoriesManager _repositoriesManager;
    private readonly IMessageFactory _messageFactory;

    public MessageService(IRepositoriesManager repositoriesManager)
    {
        ArgumentNullException.ThrowIfNull(repositoriesManager);
        _repositoriesManager = repositoriesManager;
        _messageFactory = new EmailMessageFactory();
        _messageFactory
            .AddNext(new MessengerMessageFactory())
            .AddNext(new PhoneMessageFactory());
    }

    public async Task<MessageDto> SendMessage(int senderSourceId, int recipientSourceId, string text)
    {
        EmptyStringBadQueryException.ThrowIfStringNull(text);

        using var transaction = await _repositoriesManager.MessageRepository.BeginTransactionAsync();

        var senderSource =
            await _repositoriesManager.MessageSourceRepository.GetMessageSourceAsync(senderSourceId, transaction) ??
            throw new FieldNotFoundException();
        var recipientSource =
            await _repositoriesManager.MessageSourceRepository.GetMessageSourceAsync(recipientSourceId, transaction) ??
            throw new FieldNotFoundException();

        int messageId = await _repositoriesManager.MessageRepository.InsertMessageAsync(
                            _messageFactory.CreateMessage(senderSource, recipientSource, text), transaction) ??
                        throw new FieldNotFoundException();

        var message = await _repositoriesManager.MessageRepository.GetMessageAsync(messageId, transaction) ??
                      throw new FieldNotFoundException();

        var sourceAnalytics = await _repositoriesManager.SourceAnalyticsRepository
            .GetSourceAnalyticsByMessageSourceIdAndDateAsync(
                recipientSourceId, DateOnly.FromDateTime(message.Date), transaction);
        if (sourceAnalytics is null)
        {
            await _repositoriesManager.SourceAnalyticsRepository.InsertSourceAnalyticsAsync(new SourceAnalytics
            {
                MessageSourceId = recipientSourceId,
                MessagesCount = 1,
                SessionDate = DateOnly.FromDateTime(DateTime.Today)
            });
        }
        else
        {
            sourceAnalytics.MessagesCount++;
            await _repositoriesManager.SourceAnalyticsRepository.UpdateSourceAnalyticsAsync(sourceAnalytics,
                transaction);
        }

        transaction.Commit();
        transaction.Connection?.Close();

        return new MessageDto(messageId, text, senderSourceId, recipientSourceId, false, senderSource.SourceType,
            message.Date);
    }

    public async Task CheckMessage(int sessionId, int messageId)
    {
        using var transaction = await _repositoriesManager.MessageRepository.BeginTransactionAsync();

        var session = await _repositoriesManager.SessionRepository.GetSessionAsync(sessionId, transaction) ??
                      throw new FieldNotFoundException();
        var user = await _repositoriesManager.WorkerRepository.GetWorkerAsync(session.WorkerId, transaction) ??
                   throw new FieldNotFoundException();
        var message = await _repositoriesManager.MessageRepository.GetMessageAsync(messageId, transaction) ??
                      throw new FieldNotFoundException();
        if (user.MessageSources.All(x => x.IncomingMessages.All(m => m.Id != messageId)))
            throw new FieldNotFoundException();
        if (message.Checked)
            throw new BadQueryException("Message already checked");

        message.Checked = true;
        await _repositoriesManager.MessageRepository.UpdateMessageAsync(message, transaction);

        var sessionAnalytics =
            await _repositoriesManager.SessionAnalyticsRepository.GetSessionAnalyticsAsync(session.AnalyticsId,
                transaction) ?? throw new FieldNotFoundException();
        sessionAnalytics.CheckedMessages++;
        await _repositoriesManager.SessionAnalyticsRepository.UpdateSessionAnalyticsAsync(sessionAnalytics,
            transaction);

        transaction.Commit();
        transaction.Connection?.Close();
    }
}