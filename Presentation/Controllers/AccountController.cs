using Application.DTO;
using Application.DTO.UnregisteredWorkers;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[ApiController]
[Route("[controller]")]
public class AccountController : ControllerBase
{
    private readonly IServiceManager _serviceManager;

    public AccountController(IServiceManager serviceManager)
    {
        _serviceManager = serviceManager;
    }

    [HttpPost]
    [Route("Create")]
    public async Task<AccountDto> CreateAccount(int sessionId, [FromBody] AccountDto accountDto)
    {
        try
        {
            return await _serviceManager.AccountService.Create(sessionId, accountDto);
        }
        catch (Exception e)
        {
            BadRequest(e);
            throw;
        }
    }
    
    [HttpPost]
    [Route("Update")]
    public async Task<AccountDto> UpdateAccount(int sessionId, [FromBody] AccountDto accountDto)
    {
        try
        {
            return await _serviceManager.AccountService.Update(sessionId, accountDto);
        }
        catch (Exception e)
        {
            BadRequest(e);
            throw;
        }
    }

    [HttpPost]
    [Route("Delete")]
    public async Task DeleteAccount(int sessionId, int workerId)
    {
        try
        {
            await _serviceManager.AccountService.Delete(sessionId, workerId);
            Ok("Deleted");
        }
        catch (Exception e)
        {
            BadRequest(e);
            throw;
        }
    }

    [HttpGet]
    [Route("GetByWorkerId")]
    public async Task<AccountDto> GetAccountById(int sessionId, int workerId)
    {
        try
        {
            return await _serviceManager.AccountService.GetByWorkerId(sessionId, workerId);
        }
        catch (Exception e)
        {
            BadRequest(e);
            throw;
        }
    }
    
    [HttpGet]
    [Route("GetByLogin")]
    public async Task<AccountDto> GetAccountByLogin(int sessionId, string login)
    {
        try
        {
            return await _serviceManager.AccountService.GetByLogin(sessionId, login);
        }
        catch (Exception e)
        {
            BadRequest(e);
            throw;
        }
    }

    [HttpGet]
    [Route("GetAll")]
    public async Task<IEnumerable<AccountDto>> GetAllAccounts(int sessionId)
    {
        try
        {
            return await _serviceManager.AccountService.GetAll(sessionId);
        }
        catch (Exception e)
        {
            BadRequest(e);
            throw;
        }
    }
}