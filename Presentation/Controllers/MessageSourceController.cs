using Application.DTO;
using Application.DTO.MessageSources;
using Application.DTO.UnregisteredMessageSources;
using Application.DTO.UnregisteredWorkers;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[ApiController]
[Route("[controller]")]
public class MessageSourceController : ControllerBase
{
    private readonly IServiceManager _serviceManager;

    public MessageSourceController(IServiceManager serviceManager)
    {
        _serviceManager = serviceManager;
    }

    [HttpPost]
    [Route("CreateExternal")]
    public async Task<MessageSourceDto> CreateExternalSource([FromBody] UnregisteredMessageSourceDto messageSourceDto)
    {
        try
        {
            return await _serviceManager.MessageSourceService.CreateExternal(messageSourceDto);
        }
        catch (Exception e)
        {
            BadRequest(e);
            throw;
        }
    }
    
    [HttpPost]
    [Route("CreateEmailForWorker")]
    public async Task<MessageSourceDto> CreateWorkerSource(int sessionId, int workerId, [FromBody] UnregisteredEmailSourceDto messageSourceDto)
    {
        try
        {
            return await _serviceManager.MessageSourceService.AddForWorker(sessionId, workerId, messageSourceDto);
        }
        catch (Exception e)
        {
            BadRequest(e);
            throw;
        }
    }

    [HttpPost]
    [Route("CreateMessengerForWorker")]
    public async Task<MessageSourceDto> CreateWorkerSource(int sessionId, int workerId, [FromBody] UnregisteredMessengerSourceDto messageSourceDto)
    {
        try
        {
            return await _serviceManager.MessageSourceService.AddForWorker(sessionId, workerId, messageSourceDto);
        }
        catch (Exception e)
        {
            BadRequest(e);
            throw;
        }
    }

    [HttpPost]
    [Route("CreatePhoneForWorker")]
    public async Task<MessageSourceDto> CreateWorkerSource(int sessionId, int workerId, [FromBody] UnregisteredPhoneSourceDto messageSourceDto)
    {
        try
        {
            return await _serviceManager.MessageSourceService.AddForWorker(sessionId, workerId, messageSourceDto);
        }
        catch (Exception e)
        {
            BadRequest(e);
            throw;
        }
    }

    [HttpPost]
    [Route("UpdateEmailForWorker")]
    public async Task<MessageSourceDto> UpdateWorkerSource(int sessionId, int workerId, int sourceId, [FromBody] UnregisteredEmailSourceDto messageSourceDto)
    {
        try
        {
            return await _serviceManager.MessageSourceService.Update(sessionId, workerId, sessionId, messageSourceDto);
        }
        catch (Exception e)
        {
            BadRequest(e);
            throw;
        }
    }

    [HttpPost]
    [Route("UpdateMessengerForWorker")]
    public async Task<MessageSourceDto> UpdateWorkerSource(int sessionId, int workerId, int sourceId, [FromBody] UnregisteredMessengerSourceDto messageSourceDto)
    {
        try
        {
            return await _serviceManager.MessageSourceService.Update(sessionId, workerId, sessionId, messageSourceDto);
        }
        catch (Exception e)
        {
            BadRequest(e);
            throw;
        }
    }
    
    [HttpPost]
    [Route("UpdatePhoneForWorker")]
    public async Task<MessageSourceDto> UpdateWorkerSource(int sessionId, int workerId, int sourceId, [FromBody] UnregisteredPhoneSourceDto messageSourceDto)
    {
        try
        {
            return await _serviceManager.MessageSourceService.Update(sessionId, workerId, sessionId, messageSourceDto);
        }
        catch (Exception e)
        {
            BadRequest(e);
            throw;
        }
    }

    [HttpPost]
    [Route("DeleteForWorker")]
    public async Task DeleteWorkerAccount(int sessionId, int workerId, int sourceId)
    {
        try
        {
            await _serviceManager.MessageSourceService.DeleteForWorker(sessionId, workerId, sourceId);
            Ok("Deleted");
        }
        catch (Exception e)
        {
            BadRequest(e);
            throw;
        }
    }

    [HttpGet]
    [Route("GetById")]
    public async Task<MessageSourceDto> GetSourceById(int sessionId, int sourceId)
    {
        try
        {
            return await _serviceManager.MessageSourceService.GetById(sessionId, sourceId);
        }
        catch (Exception e)
        {
            BadRequest(e);
            throw;
        }
    }
    
    [HttpGet]
    [Route("GetByWorkerId")]
    public async Task<IEnumerable<MessageSourceDto>> GetSourceByWorkerId(int sessionId, int workerId)
    {
        try
        {
            return await _serviceManager.MessageSourceService.GetByWorkerId(sessionId, workerId);
        }
        catch (Exception e)
        {
            BadRequest(e);
            throw;
        }
    }
    
    [HttpGet]
    [Route("GetAll")]
    public async Task<IEnumerable<MessageSourceDto>> GetAllAccounts(int sessionId)
    {
        try
        {
            return await _serviceManager.MessageSourceService.GetAll(sessionId);
        }
        catch (Exception e)
        {
            BadRequest(e);
            throw;
        }
    }
}