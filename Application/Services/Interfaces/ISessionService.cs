using Application.Commands.Workers;
using Application.DTO;
using Application.Utils.WorkerDtoExecutors;
using Domain.Models;
using Domain.Models.Workers;

namespace Application.Services.Interfaces;

public interface ISessionService
{
    Task<SessionDto> Start(UnregisteredAccountDto accountDto); 
    Task Finish(int sessionId);
    Task<SessionDto> GetById(int sessionId, int id);
    Task<SessionDto> GetByWorkerId(int sessionId, int workerId);
    Task<IEnumerable<SessionDto>> GetAll(int sessionId);
}