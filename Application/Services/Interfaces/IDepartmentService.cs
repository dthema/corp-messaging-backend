using Application.DTO;
using Domain.Models;

namespace Application.Services.Interfaces;

public interface IDepartmentService
{
    Task<DepartmentDto> Create(int sessionId, string departmentName);
    Task<DepartmentDto> Update(int sessionId, RegisteredDepartmentDto departmentDto);
    Task Delete(int sessionId, int departmentId);
    Task<DepartmentDto> GetById(int sessionId, int departmentId);
    Task<IEnumerable<DepartmentDto>> GetAll(int sessionId);
}