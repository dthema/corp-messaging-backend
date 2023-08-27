using Application.Commands.Workers;
using Application.DTO.UnregisteredWorkers;
using Application.DTO.Workers;
using Application.Utils.WorkerDtoExecutors;
using Domain.Models;
using Domain.Models.Workers;

namespace Application.Services.Interfaces;

public interface IWorkerService
{
    Task<WorkerDto> Create(int sessionId, UnregisteredWorkerDto workerDto); 
    Task<WorkerDto> Update(int sessionId, int workerId, UnregisteredWorkerDto workerDto);
    Task Delete(int sessionId, int workerId);
    Task<WorkerDto> GetById(int sessionId, int workerId);
    Task<IEnumerable<WorkerDto>> GetByDepartmentId(int sessionId, int departmentId);
    Task<IEnumerable<WorkerDto>> GetByRole(int sessionId, Role role);
    Task<IEnumerable<WorkerDto>> GetAll(int sessionId);
}