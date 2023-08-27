namespace Domain.Repositories;

public interface IRepositoriesManager
{
    IAccountRepository AccountRepository { get; }
    IDepartmentRepository DepartmentRepository { get; }
    IMessageRepository MessageRepository { get; }
    IMessageSourceRepository MessageSourceRepository { get; }
    IReportRepository ReportRepository { get; }
    ISessionRepository SessionRepository { get; }
    ISessionAnalyticsRepository SessionAnalyticsRepository { get; }
    ISourceAnalyticsRepository SourceAnalyticsRepository { get; }
    IWorkerRepository WorkerRepository { get; }
    IWorkerSourcesRepository WorkerSourcesRepository { get; }
}