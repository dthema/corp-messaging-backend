using System.Data;
using Domain.Models;

namespace Domain.Repositories;

public interface IDepartmentRepository : IBaseRepository
{
    Task<int?> InsertDepartmentAsync(Department department, IDbTransaction? transaction = null);
    Task<int> DeleteDepartmentAsync(int id, IDbTransaction? transaction = null);
    Task DeleteAllDepartmentsAsync(IDbTransaction? transaction = null);
    Task<int> UpdateDepartmentAsync(Department department, IDbTransaction? transaction = null);
    Task<Department> GetDepartmentAsync(int id, IDbTransaction? transaction = null);
    Task<IEnumerable<Department>> GetAllDepartmentsAsync(IDbTransaction? transaction = null);
}