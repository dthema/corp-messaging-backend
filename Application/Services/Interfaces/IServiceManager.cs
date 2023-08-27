namespace Application.Services.Interfaces;

public interface IServiceManager
{
    IAccountService AccountService { get; }
    IDepartmentService DepartmentService { get; }
    IInitializationService InitializationService { get; }
    IMessageService MessageService { get; }
    IMessageSourceService MessageSourceService { get; }
    IReportService ReportService { get; }
    ISessionService SessionService { get; }
    IWorkerService WorkerService { get; }
}