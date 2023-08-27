using Dapper;
using Domain.Repositories;
using Infrastructure.TypeHandlers;

namespace Infrastructure.Repositories;

public class RepositoriesManager : IRepositoriesManager
{
    private readonly Lazy<IAccountRepository> _lazyAccountRepository;
    private readonly Lazy<IDepartmentRepository> _lazyDepartmentRepository;
    private readonly Lazy<IMessageRepository> _lazyMessageRepository;
    private readonly Lazy<IMessageSourceRepository> _lazyMessageSourceRepository;
    private readonly Lazy<IReportRepository> _lazyReportRepository;
    private readonly Lazy<ISessionRepository> _lazySessionRepository;
    private readonly Lazy<ISessionAnalyticsRepository> _lazySessionAnalyticsRepository;
    private readonly Lazy<ISourceAnalyticsRepository> _lazySourceAnalyticsRepository;
    private readonly Lazy<IWorkerRepository> _lazyWorkerRepository;
    private readonly Lazy<IWorkerSourcesRepository> _lazyWorkerSourceRepository;

    public RepositoriesManager()
    {
        SqlMapper.AddTypeHandler(new DateOnlyTypeHandler());
        _lazyAccountRepository = new Lazy<IAccountRepository>(() => new AccountRepository());
        _lazyDepartmentRepository = new Lazy<IDepartmentRepository>(() => new DepartmentRepository());
        _lazyMessageRepository = new Lazy<IMessageRepository>(() => new MessageRepository());
        _lazyMessageSourceRepository = new Lazy<IMessageSourceRepository>(() => new MessageSourceRepository());
        _lazyReportRepository = new Lazy<IReportRepository>(() => new ReportRepository());
        _lazySessionRepository = new Lazy<ISessionRepository>(() => new SessionRepository());
        _lazySessionAnalyticsRepository = new Lazy<ISessionAnalyticsRepository>(() => new SessionAnalyticsRepository());
        _lazySourceAnalyticsRepository = new Lazy<ISourceAnalyticsRepository>(() => new SourceAnalyticsRepository());
        _lazyWorkerRepository = new Lazy<IWorkerRepository>(() => new WorkerRepository());
        _lazyWorkerSourceRepository = new Lazy<IWorkerSourcesRepository>(() => new WorkerSourcesRepository());
    }

    public IAccountRepository AccountRepository => _lazyAccountRepository.Value;
    public IDepartmentRepository DepartmentRepository => _lazyDepartmentRepository.Value;
    public IMessageRepository MessageRepository => _lazyMessageRepository.Value;
    public IMessageSourceRepository MessageSourceRepository => _lazyMessageSourceRepository.Value;
    public IReportRepository ReportRepository => _lazyReportRepository.Value;
    public ISessionRepository SessionRepository => _lazySessionRepository.Value;
    public ISessionAnalyticsRepository SessionAnalyticsRepository => _lazySessionAnalyticsRepository.Value;
    public ISourceAnalyticsRepository SourceAnalyticsRepository => _lazySourceAnalyticsRepository.Value;
    public IWorkerRepository WorkerRepository => _lazyWorkerRepository.Value;
    public IWorkerSourcesRepository WorkerSourcesRepository => _lazyWorkerSourceRepository.Value;
}