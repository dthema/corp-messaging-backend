using Application.Services.Interfaces;
using Domain.Repositories;

namespace Application.Services;

public class ServiceManager : IServiceManager
{
    private readonly Lazy<IAccountService> _lazyAccountService;
    private readonly Lazy<IDepartmentService> _lazyDepartmentService;
    private readonly Lazy<IInitializationService> _lazyInitializationService;
    private readonly Lazy<IMessageService> _lazyMessageService;
    private readonly Lazy<IMessageSourceService> _lazyMessageSourceService;
    private readonly Lazy<IReportService> _lazyReportService;
    private readonly Lazy<ISessionService> _lazySessionService;
    private readonly Lazy<IWorkerService> _lazyWorkerService;

    public ServiceManager(IRepositoriesManager repositoriesManager)
    {
        _lazyAccountService = new Lazy<IAccountService>(() => new AccountService(repositoriesManager));
        _lazyDepartmentService = new Lazy<IDepartmentService>(() => new DepartmentService(repositoriesManager));
        _lazyInitializationService = new Lazy<IInitializationService>(() => new InitializationService(repositoriesManager));
        _lazyMessageService = new Lazy<IMessageService>(() => new MessageService(repositoriesManager));
        _lazyMessageSourceService = new Lazy<IMessageSourceService>(() => new MessageSourceService(repositoriesManager));
        _lazyReportService = new Lazy<IReportService>(() => new ReportService(repositoriesManager));
        _lazySessionService = new Lazy<ISessionService>(() => new SessionService(repositoriesManager));
        _lazyWorkerService = new Lazy<IWorkerService>(() => new WorkerService(repositoriesManager));
    }

    public IAccountService AccountService => _lazyAccountService.Value;
    public IDepartmentService DepartmentService => _lazyDepartmentService.Value;
    public IInitializationService InitializationService => _lazyInitializationService.Value;
    public IMessageService MessageService => _lazyMessageService.Value;
    public IMessageSourceService MessageSourceService => _lazyMessageSourceService.Value;
    public IReportService ReportService => _lazyReportService.Value;
    public ISessionService SessionService => _lazySessionService.Value;
    public IWorkerService WorkerService => _lazyWorkerService.Value;
}