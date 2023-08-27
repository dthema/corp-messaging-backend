using Application.DTO;
using Application.DTO.UnregisteredWorkers;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[ApiController]
[Route("[controller]")]
public class InitializationController : ControllerBase
{
    private readonly IServiceManager _serviceManager;

    public InitializationController(IServiceManager serviceManager)
    {
        _serviceManager = serviceManager;
    }

    [HttpPost]
    [Route("CreateChiefWithAccount")]
    public async Task<ChiefAccountDto> CreateChiefWithAccount([FromBody] UnregisteredChiefAccountDto chiefAccountDto)
    {
        return await _serviceManager.InitializationService.CreateChiefWithAccount(chiefAccountDto);
    }
}