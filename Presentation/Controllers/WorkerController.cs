using Application.DTO;
using Application.DTO.UnregisteredWorkers;
using Application.DTO.Workers;
using Application.Services.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[ApiController]
[Route("[controller]")]
public class WorkerController : ControllerBase
{
    private readonly IServiceManager _serviceManager;

    public WorkerController(IServiceManager serviceManager)
    {
        _serviceManager = serviceManager;
    }

    [HttpPost]
    [Route("AddEmployee")]
    public async Task<WorkerDto> AddEmployee(int sessionId, [FromBody] UnregisteredEmployeeDto employeeDto)
    {
        try
        {
            return await _serviceManager.WorkerService.Create(sessionId, employeeDto);
        }
        catch (Exception e)
        {
            BadRequest(e);
            throw;
        }
    }
    
    [HttpPost]
    [Route("AddManager")]
    public async Task<WorkerDto> AddManager(int sessionId, [FromBody] UnregisteredManagerDto managerDto)
    {
        try
        {
            return await _serviceManager.WorkerService.Create(sessionId, managerDto);
        }
        catch (Exception e)
        {
            BadRequest(e);
            throw;
        }
    }

    [HttpPost]
    [Route("UpdateEmployee")]
    public async Task<WorkerDto> UpdateEmployee(int sessionId, int workerId, [FromBody] UnregisteredEmployeeDto employeeDto)
    {
        try
        {
            return await _serviceManager.WorkerService.Update(sessionId, workerId, employeeDto);
        }
        catch (Exception e)
        {
            BadRequest(e);
            throw;
        }
    }

    [HttpPost]
    [Route("UpdateManager")]
    public async Task<WorkerDto> UpdateManager(int sessionId, int workerId, [FromBody] UnregisteredManagerDto managerDto)
    {
        try
        {
            return await _serviceManager.WorkerService.Update(sessionId, workerId, managerDto);
        }
        catch (Exception e)
        {
            BadRequest(e);
            throw;
        }
    }

    [HttpPost]
    [Route("Delete")]
    public async Task DeleteWorker(int sessionId, int workerId)
    {
        try
        {
            await _serviceManager.WorkerService.Delete(sessionId, workerId);
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
    public async Task<WorkerDto> GetWorkerById(int sessionId, int workerId)
    {
        try
        {
            return await _serviceManager.WorkerService.GetById(sessionId, workerId);
        }
        catch (Exception e)
        {
            BadRequest(e);
            throw;
        }
    }

    [HttpGet]
    [Route("GetByDepartment")]
    public async Task<IEnumerable<WorkerDto>> GetWorkersByDepartmentId(int sessionId, [FromBody] int departmentId)
    {
        try
        {
            return await _serviceManager.WorkerService.GetByDepartmentId(sessionId, departmentId);
        }
        catch (Exception e)
        {
            BadRequest(e);
            throw;
        }
    }

    [HttpGet]
    [Route("GetByRole")]
    public async Task<IEnumerable<WorkerDto>> GetWorkersByRole(int sessionId, [FromBody] Role role)
    {
        try
        {
            return await _serviceManager.WorkerService.GetByRole(sessionId, role);
        }
        catch (Exception e)
        {
            BadRequest(e);
            throw;
        }
    }
    
    [HttpGet]
    [Route("GetAll")]
    public async Task<IEnumerable<WorkerDto>> GetAllWorkers(int sessionId)
    {
        try
        {
            return await _serviceManager.WorkerService.GetAll(sessionId);
        }
        catch (Exception e)
        {
            BadRequest(e);
            throw;
        }
    }
}