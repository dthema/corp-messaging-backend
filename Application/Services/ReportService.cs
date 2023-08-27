using System.Data;
using System.Net;
using Application.DTO;
using Application.DTO.MessageSources;
using Application.Services.Interfaces;
using Application.Utils.MessageSourceFactories;
using Domain.Exceptions;
using Domain.Models;
using Domain.Models.Workers;
using Domain.Repositories;

namespace Application.Services;

public class ReportService : IReportService
{
    private readonly IRepositoriesManager _repositoriesManager;
    private readonly IMessageSourceFactory _messageSourceFactory;

    public ReportService(IRepositoriesManager repositoriesManager)
    {
        ArgumentNullException.ThrowIfNull(repositoriesManager);
        _repositoriesManager = repositoriesManager;
        _messageSourceFactory = new EmailSourceFactory();
        _messageSourceFactory
            .AddNext(new MessengerSourceFactory())
            .AddNext(new PhoneSourceFactory());
    }

    public async Task<ReportDto> CreateReportForDepartment(int sessionId, int departmentId, DateOnly date)
    {
        using var transaction = await _repositoriesManager.ReportRepository.BeginTransactionAsync();

        var session = await _repositoriesManager.SessionRepository.GetSessionAsync(sessionId, transaction) ??
                      throw new FieldNotFoundException();
        var user = await _repositoriesManager.WorkerRepository.GetWorkerAsync(session.WorkerId, transaction) ??
                   throw new FieldNotFoundException();
        if (user.Role == Role.Employee)
            throw new BadQueryException("Employee cannot manage reports");
        if (user is Manager manager && manager.DepartmentId != departmentId)
            throw new BadQueryException("Manager cannot manage reports from different department");

        var existedReport =
            await _repositoriesManager.ReportRepository.GetReportByDepartmentIdAndDateAsync(departmentId, date,
                transaction);
        if (existedReport is not null)
            throw new BadQueryException("Report already existed");

        var sessionsAnalytics =
            await _repositoriesManager.SessionAnalyticsRepository.GetSessionsAnalyticsByDepartmentIdAndDateAsync(
                departmentId, date, transaction);
        var sourcesAnalytics =
            await _repositoriesManager.SourceAnalyticsRepository.GetSourcesAnalyticsByDepartmentIdAndDateAsync(
                departmentId, date, transaction);

        var sources = await _repositoriesManager.MessageSourceRepository.GetAllMessageSourcesAsync(transaction);
        var messageSources = sources.Where(x => sourcesAnalytics.Any(analytics => analytics.MessageSourceId == x.Id));
        var sourcesList = messageSources.ToList();
        var messagesByDate = sourcesList
            .SelectMany(x => x.IncomingMessages)
            .ToList();
        messagesByDate.AddRange(sourcesList.SelectMany(x => x.OutgoingMessages));

        var reportId = await _repositoriesManager.ReportRepository.InsertReportAsync(new Report
        {
            DepartmentId = departmentId,
            CheckedMessages = sessionsAnalytics.Sum(x => x.CheckedMessages),
            MessagesByDate = messagesByDate.Count,
            Date = date,
        }, transaction) ?? throw new FieldNotFoundException();
        var report = await _repositoriesManager.ReportRepository.GetReportAsync(reportId, transaction);

        var reportDto = await GetReportDto(report, transaction);

        transaction.Commit();
        transaction.Connection?.Close();

        return reportDto;
    }

    public async Task<ReportDto> CreateGlobalReport(int sessionId, DateOnly date)
    {
        using var transaction = await _repositoriesManager.ReportRepository.BeginTransactionAsync();

        var session = await _repositoriesManager.SessionRepository.GetSessionAsync(sessionId, transaction) ??
                      throw new FieldNotFoundException();
        var user = await _repositoriesManager.WorkerRepository.GetWorkerAsync(session.WorkerId, transaction) ??
                   throw new FieldNotFoundException();
        if (user.Role != Role.Chief)
            throw new BadQueryException("Only cheafs can create global reports");

        var existedReport =
            await _repositoriesManager.ReportRepository.GetReportByDepartmentIdAndDateAsync(0, date, transaction);
        if (existedReport is not null)
            throw new BadQueryException("Report already existed");

        var sessionsAnalytics =
            await _repositoriesManager.SessionAnalyticsRepository.GetSessionsAnalyticsByDateAsync(date, transaction);
        var sourcesAnalytics =
            await _repositoriesManager.SourceAnalyticsRepository.GetSourcesAnalyticsByDateAsync(date, transaction);

        var sources = await _repositoriesManager.MessageSourceRepository.GetAllMessageSourcesAsync(transaction);
        var messageSources = sources.Where(x => sourcesAnalytics.Any(analytics => analytics.MessageSourceId == x.Id));
        var sourcesList = messageSources.ToList();
        var messagesByDate = sourcesList
            .SelectMany(x => x.IncomingMessages)
            .ToList();
        messagesByDate.AddRange(sourcesList.SelectMany(x => x.OutgoingMessages));

        var reportId = await _repositoriesManager.ReportRepository.InsertReportAsync(new Report
        {
            CheckedMessages = sessionsAnalytics.Sum(x => x.CheckedMessages),
            MessagesByDate = messagesByDate.Count,
            Date = date,
        }, transaction) ?? throw new FieldNotFoundException();
        var report = await _repositoriesManager.ReportRepository.GetReportAsync(reportId, transaction);

        var reportDto = await GetReportDto(report, transaction);

        transaction.Commit();
        transaction.Connection?.Close();

        return reportDto;
    }

    public async Task Delete(int sessionId, int reportId)
    {
        using var transaction = await _repositoriesManager.ReportRepository.BeginTransactionAsync();

        var session = await _repositoriesManager.SessionRepository.GetSessionAsync(sessionId, transaction) ??
                      throw new FieldNotFoundException();
        var user = await _repositoriesManager.WorkerRepository.GetWorkerAsync(session.WorkerId, transaction) ??
                   throw new FieldNotFoundException();
        if (user.Role != Role.Chief)
            throw new BadQueryException("Only cheafs can delete reports");

        var existedReport = await _repositoriesManager.ReportRepository.GetReportAsync(reportId, transaction) ??
                            throw new FieldNotFoundException();
        await _repositoriesManager.ReportRepository.DeleteReportAsync(reportId, transaction);

        transaction.Commit();
        transaction.Connection?.Close();
    }

    public async Task<ReportDto> GetById(int sessionId, int reportId)
    {
        using var transaction = await _repositoriesManager.ReportRepository.BeginTransactionAsync();

        var session = await _repositoriesManager.SessionRepository.GetSessionAsync(sessionId, transaction) ??
                      throw new FieldNotFoundException();
        var user = await _repositoriesManager.WorkerRepository.GetWorkerAsync(session.WorkerId, transaction) ??
                   throw new FieldNotFoundException();
        if (user.Role != Role.Chief)
            throw new BadQueryException("Only cheafs can get reports by id");

        var report = await _repositoriesManager.ReportRepository.GetReportAsync(reportId, transaction) ??
                     throw new FieldNotFoundException();

        var reportDto = await GetReportDto(report, transaction);

        transaction.Commit();
        transaction.Connection?.Close();

        return reportDto;
    }

    public async Task<IEnumerable<ReportDto>> GetByDepartmentId(int sessionId, int departmentId)
    {
        using var transaction = await _repositoriesManager.ReportRepository.BeginTransactionAsync();

        var session = await _repositoriesManager.SessionRepository.GetSessionAsync(sessionId, transaction) ??
                      throw new FieldNotFoundException();
        var user = await _repositoriesManager.WorkerRepository.GetWorkerAsync(session.WorkerId, transaction) ??
                   throw new FieldNotFoundException();
        if (user.Role == Role.Employee)
            throw new BadQueryException("Employee cannot manage reports");
        if (user is Manager manager && manager.DepartmentId != departmentId)
            throw new BadQueryException("Manager cannot manage reports from different department");

        var reports =
            await _repositoriesManager.ReportRepository.GetReportsByDepartmentIdAsync(departmentId, transaction) ??
            throw new FieldNotFoundException();
        reports = reports.ToList();
        var reportsDto = new List<ReportDto>();

        foreach (Report report in reports)
            reportsDto.Add(await GetReportDto(report, transaction));

        transaction.Commit();
        transaction.Connection?.Close();

        return reportsDto;
    }

    public async Task<IEnumerable<ReportDto>> GetAll(int sessionId)
    {
        using var transaction = await _repositoriesManager.ReportRepository.BeginTransactionAsync();

        var session = await _repositoriesManager.SessionRepository.GetSessionAsync(sessionId, transaction) ??
                      throw new FieldNotFoundException();
        var user = await _repositoriesManager.WorkerRepository.GetWorkerAsync(session.WorkerId, transaction) ??
                   throw new FieldNotFoundException();
        if (user.Role != Role.Chief)
            throw new BadQueryException("Only cheafs can get all reports");

        var reports = await _repositoriesManager.ReportRepository.GetAllReportsAsync(transaction) ??
                      throw new FieldNotFoundException();
        reports = reports.ToList();
        var reportsDto = new List<ReportDto>();

        foreach (Report report in reports)
            reportsDto.Add(await GetReportDto(report, transaction));

        transaction.Commit();
        transaction.Connection?.Close();

        return reportsDto;
    }

    private async Task<ReportDto> GetReportDto(Report report, IDbTransaction transaction)
    {
        var sourcesDto = new List<MessageSourceDto>();
        foreach (SourceAnalytics sourceAnalytics in report.SourcesAnalytics)
        {
            var messageSource =
                await _repositoriesManager.MessageSourceRepository.GetMessageSourceAsync(
                    sourceAnalytics.MessageSourceId, transaction);
            if (messageSource is null)
                continue;
            sourcesDto.Add(_messageSourceFactory.CreateMessageSourceDto(messageSource));
        }

        return new ReportDto(
            report.Id,
            report.CheckedMessages,
            report.SourcesAnalytics.Select(x =>
            {
                return new SourceAnalyticsDto(
                    x.Id,
                    sourcesDto.FirstOrDefault(dto => dto.Id == x.MessageSourceId) ?? throw new FieldNotFoundException(),
                    x.MessagesCount);
            }));
    }
}