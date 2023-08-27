using Application.DTO.UnregisteredWorkers;

namespace Application.Utils.WorkerDtoExecutors;

public interface IWorkerDtoExecutor
{
    Task<int?> DoWorkerDtoCommand(UnregisteredWorkerDto workerDto);
    IWorkerDtoExecutor AddNext(IWorkerDtoExecutor workerDtoExecutor);
}