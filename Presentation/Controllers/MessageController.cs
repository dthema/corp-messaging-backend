using Application.DTO;
using Application.DTO.Messages;
using Application.DTO.UnregisteredWorkers;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Presentation.DTO;

namespace Presentation.Controllers;

[ApiController]
[Route("[controller]")]
public class MessageController : ControllerBase
{
    private readonly IServiceManager _serviceManager;

    public MessageController(IServiceManager serviceManager)
    {
        _serviceManager = serviceManager;
    }

    [HttpPost]
    [Route("Send")]
    public async Task<MessageDto> SendMessage([FromBody] MessageSendDto sendDto)
    {
        try
        {
            return await _serviceManager.MessageService.SendMessage(sendDto.SenderSourceId, sendDto.RecipientSourceId, sendDto.Text);
        }
        catch (Exception e)
        {
            BadRequest(e);
            throw;
        }
    }
    
    [HttpPost]
    [Route("Check")]
    public async Task CheckMessage(int sessionId, int messageId)
    {
        try
        {
            await _serviceManager.MessageService.CheckMessage(sessionId, messageId);
            Ok("Checked");
        }
        catch (Exception e)
        {
            BadRequest(e);
            throw;
        }
    }
}