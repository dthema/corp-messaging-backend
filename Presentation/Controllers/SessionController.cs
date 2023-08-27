using Application.DTO;
using Application.DTO.UnregisteredWorkers;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[ApiController]
[Route("[controller]")]
public class SessionController : ControllerBase
{
    private readonly IServiceManager _serviceManager;

    public SessionController(IServiceManager serviceManager)
    {
        _serviceManager = serviceManager;
    }

    [HttpPost]
    [Route("Start")]
    public async Task<SessionDto> StartSession([FromBody] UnregisteredAccountDto accountDto)
    {
        try
        {
            return await _serviceManager.SessionService.Start(accountDto);
        }
        catch (Exception e)
        {
            BadRequest(e);
            throw;
        }
    }
    
    [HttpPost]
    [Route("Finish")]
    public async Task FinishSession(int sessionId)
    {
        try
        {
            await _serviceManager.SessionService.Finish(sessionId);
            Ok("Finished");
        }
        catch (Exception e)
        {
            BadRequest(e);
            throw;
        }
    }

    [HttpGet]
    [Route("GetById")]
    public async Task<SessionDto> GetSessionById(int currentSessionId, int requiredSessionId)
    {
        try
        {
            return await _serviceManager.SessionService.GetById(currentSessionId, requiredSessionId);
        }
        catch (Exception e)
        {
            BadRequest(e);
            throw;
        }
    }
    
    [HttpGet]
    [Route("GetByWorkerId")]
    public async Task<SessionDto> GetSessionByWorkerId(int currentSessionId, int workerId)
    {
        try
        {
            return await _serviceManager.SessionService.GetByWorkerId(currentSessionId, workerId);
        }
        catch (Exception e)
        {
            BadRequest(e);
            throw;
        }
    }
    
    [HttpGet]
    [Route("GetAll")]
    public async Task<IEnumerable<SessionDto>> GetAllAccounts(int sessionId)
    {
        try
        {
            return await _serviceManager.SessionService.GetAll(sessionId);
        }
        catch (Exception e)
        {
            BadRequest(e);
            throw;
        }
    }
}