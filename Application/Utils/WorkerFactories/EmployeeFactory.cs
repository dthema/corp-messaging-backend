using Application.DTO.Workers;
using Application.Utils.MessageSourceFactories;
using Domain.Models.Workers;

namespace Application.Utils.WorkerFactories;

public class EmployeeFactory : IWorkerFactory
{
    private readonly IMessageSourceFactory _messageSourceFactory;
    private IWorkerFactory? _nextFactory;

    public EmployeeFactory()
    {
        _messageSourceFactory = new EmailSourceFactory();
        _messageSourceFactory
            .AddNext(new MessengerSourceFactory())
            .AddNext(new PhoneSourceFactory());
    }
    
    public WorkerDto CreateWorkerDto(Worker worker)
    {
        if (worker is Employee employee)
            return new EmployeeDto(employee.Id, employee.Firstname, employee.Lastname, employee.DepartmentId, worker.MessageSources.Select(messageSource => 
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