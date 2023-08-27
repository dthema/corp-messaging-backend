using Application.DTO;
using Domain.Models;
using Domain.Models.Workers;

namespace Application.Services.Interfaces;

public interface IReportService
{
    Task<ReportDto> CreateReportForDepartment(int sessionId, int departmentId, DateOnly date); 
    Task<ReportDto> CreateGlobalReport(int sessionId, DateOnly date);
    Task Delete(int sessionId, int reportId);
    Task<ReportDto> GetById(int sessionId, int reportId);
    Task<IEnumerable<ReportDto>> GetByDepartmentId(int sessionId, int departmentId);
    Task<IEnumerable<ReportDto>> GetAll(int sessionId);
}