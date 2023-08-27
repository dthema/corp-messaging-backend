using Application.DTO.Workers;
using Domain.Models.Workers;

namespace Application.Utils.WorkerFactories;

public interface IWorkerFactory
{
    WorkerDto CreateWorkerDto(Worker worker);
    IWorkerFactory AddNext(IWorkerFactory workerFactory);
}