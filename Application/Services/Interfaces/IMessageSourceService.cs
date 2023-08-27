using Application.Commands.MessageSources;
using Application.DTO.MessageSources;
using Application.DTO.UnregisteredMessageSources;
using Application.Utils.MessageSourceDtoExecutors;
using Domain.Models;
using Domain.Models.MessageSources;
using Domain.Models.Workers;

namespace Application.Services.Interfaces;

public interface IMessageSourceService
{
    Task<MessageSourceDto> CreateExternal(UnregisteredMessageSourceDto unregisteredMessageSourceDto); 
    Task<MessageSourceDto> AddForWorker(int sessionId, int workerId, UnregisteredMessageSourceDto unregisteredMessageSourceDto);
    Task<MessageSourceDto> Update(int sessionId, int workerId, int messageSourceId, UnregisteredMessageSourceDto messageSourceDto);
    Task DeleteForWorker(int sessionId, int workerId, int messageSourceId);
    Task<MessageSourceDto> GetById(int sessionId, int sourceId);
    Task<IEnumerable<MessageSourceDto>> GetByWorkerId(int sessionId, int workerId);
    Task<IEnumerable<MessageSourceDto>> GetAll(int sessionId);
}