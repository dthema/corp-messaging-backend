using Application.DTO;
using Application.DTO.UnregisteredWorkers;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Presentation.DTO;

namespace Presentation.Controllers;

[ApiController]
[Route("[controller]")]
public class ReportController : ControllerBase
{
    private readonly IServiceManager _serviceManager;

    public ReportController(IServiceManager serviceManager)
    {
        _serviceManager = serviceManager;
    }

    [HttpPost]
    [Route("CreateForDepartment")]
    public async Task<ReportDto> CreateDepartmentReport(int sessionId, [FromBody] DepartmentReportCreateDto reportCreateDto)
    {
        try
        {
            return await _serviceManager.ReportService.CreateReportForDepartment(sessionId, reportCreateDto.DepartmentId,
                new DateOnly(reportCreateDto.Date.Year, reportCreateDto.Date.Month, reportCreateDto.Date.Day));
        }
        catch (Exception e)
        {
            BadRequest(e);
            throw;
        }
    }
    
    [HttpPost]
    [Route("CreateGlobal")]
    public async Task<ReportDto> CreateGlobalReport(int sessionId, [FromBody] DateDto date)
    {
        try
        {
            return await _serviceManager.ReportService.CreateGlobalReport(sessionId, new DateOnly(date.Year, date.Month, date.Day));
        }
        catch (Exception e)
        {
            BadRequest(e);
            throw;
        }
    }

    [HttpPost]
    [Route("Delete")]
    public async Task DeleteAccount(int sessionId, int reportId)
    {
        try
        {
            await _serviceManager.ReportService.Delete(sessionId, reportId);
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
    public async Task<ReportDto> GetReportById(int sessionId, int reportId)
    {
        try
        {
            return await _serviceManager.ReportService.GetById(sessionId, reportId);
        }
        catch (Exception e)
        {
            BadRequest(e);
            throw;
        }
    }
    
    [HttpGet]
    [Route("GetForDepartment")]
    public async Task<IEnumerable<ReportDto>> GetReportsByDepartment(int sessionId, int departmentId)
    {
        try
        {
            return await _serviceManager.ReportService.GetByDepartmentId(sessionId, departmentId);
        }
        catch (Exception e)
        {
            BadRequest(e);
            throw;
        }
    }

    [HttpGet]
    [Route("GetAll")]
    public async Task<IEnumerable<ReportDto>> GetAllReports(int sessionId)
    {
        try
        {
            return await _serviceManager.ReportService.GetAll(sessionId);
        }
        catch (Exception e)
        {
            BadRequest(e);
            throw;
        }
    }
}