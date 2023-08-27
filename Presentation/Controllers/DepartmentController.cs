using Application.DTO;
using Application.DTO.UnregisteredWorkers;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[ApiController]
[Route("[controller]")]
public class DepartmentController : ControllerBase
{
    private readonly IServiceManager _serviceManager;

    public DepartmentController(IServiceManager serviceManager)
    {
        _serviceManager = serviceManager;
    }

    [HttpPost]
    [Route("Create")]
    public async Task<DepartmentDto> CreateDepartment(int sessionId, string departmentName)
    {
        try
        {
            return await _serviceManager.DepartmentService.Create(sessionId, departmentName);
        }
        catch (Exception e)
        {
            BadRequest(e);
            throw;
        }
    }
    
    [HttpPost]
    [Route("Update")]
    public async Task<DepartmentDto> UpdateDepartment(int sessionId, [FromBody] DepartmentDto departmentDto)
    {
        try
        {
            return await _serviceManager.DepartmentService.Update(sessionId, departmentDto);
        }
        catch (Exception e)
        {
            BadRequest(e);
            throw;
        }
    }

    [HttpPost]
    [Route("Delete")]
    public async Task DeleteDepartment(int sessionId, int departmentId)
    {
        try
        {
            await _serviceManager.DepartmentService.Delete(sessionId, departmentId);
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
    public async Task<DepartmentDto> GetDepartmentById(int sessionId, int departmentId)
    {
        try
        {
            return await _serviceManager.DepartmentService.GetById(sessionId, departmentId);
        }
        catch (Exception e)
        {
            BadRequest(e);
            throw;
        }
    }
    
    [HttpGet]
    [Route("GetAll")]
    public async Task<IEnumerable<DepartmentDto>> GetAllDepartments(int sessionId)
    {
        try
        {
            return await _serviceManager.DepartmentService.GetAll(sessionId);
        }
        catch (Exception e)
        {
            BadRequest(e);
            throw;
        }
    }
}