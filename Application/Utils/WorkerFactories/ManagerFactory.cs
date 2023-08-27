using Application.DTO.Workers;
using Application.Utils.MessageSourceFactories;
using Domain.Models.Workers;

namespace Application.Utils.WorkerFactories;

public class ManagerFactory : IWorkerFactory
{
    private readonly IMessageSourceFactory _messageSourceFactory;
    private IWorkerFactory? _nextFactory;

    public ManagerFactory()
    {
        _messageSourceFactory = new EmailSourceFactory();
        _messageSourceFactory
            .AddNext(new MessengerSourceFactory())
            .AddNext(new PhoneSourceFactory());
    }
    
    public WorkerDto CreateWorkerDto(Worker worker)
    {
        if (worker is Manager manager)
            return new ManagerDto(manager.Id, manager.Firstname, manager.Lastname, manager.DepartmentId, worker.MessageSources.Select(messageSource => 
                _messageSourceFactory.CreateMessageSourceDto(messageSource)).ToList());
        if (_nextFactory is not null)
            return _nextFactory.CreateWorkerDto(worker);
        throw new Exception();
    }

    public IWorkerFactory AddNext(IWorkerFactory workerFactory)
    {
        ArgumentNullException.ThrowIfNull(workerFactory);
        _nextFactory = workerFactory;
        return workerFactory;
    }
}