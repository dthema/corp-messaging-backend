using System.Data;
using Dapper;
using Domain.Models;
using Domain.Models.Workers;
using Domain.Repositories;

namespace Infrastructure.Repositories;

public class DepartmentRepository : BaseRepository, IDepartmentRepository
{
    public DepartmentRepository()
    {
        SimpleCRUD.SetDialect(SimpleCRUD.Dialect.PostgreSQL);
    }
    
    public async Task<int?> InsertDepartmentAsync(Department department, IDbTransaction? transaction = null) => await InsertAsync(department, transaction);

    public async Task<int> DeleteDepartmentAsync(int id, IDbTransaction? transaction = null) => await DeleteAsync<Department>(id, transaction);

    public async Task DeleteAllDepartmentsAsync(IDbTransaction? transaction = null) => await DeleteAllAsync<Department>(transaction);

    public async Task<int> UpdateDepartmentAsync(Department department, IDbTransaction? transaction = null) => await UpdateAsync(department, transaction);

    public async Task<Department> GetDepartmentAsync(int id, IDbTransaction? transaction = null)
    {
        var department = await GetAsync<Department>(id, transaction);
        await FillDepartmentWorkers(department);
        return department;
    }

    public async Task<IEnumerable<Department>> GetAllDepartmentsAsync(IDbTransaction? transaction = null)
    {
        var departments = await GetListAsync<Department>(transaction);
        departments = departments.ToList();
        foreach (Department department in departments)
            await FillDepartmentWorkers(department);
        return departments;
    }

    private async Task FillDepartmentWorkers(Department department)
    {   
        var workerRepository = new WorkerRepository();
        var employees = await workerRepository.GetWorkersByDepartmentIdAsync<Employee>(department.Id);
        var managers = await workerRepository.GetWorkersByDepartmentIdAsync<Manager>(department.Id);
        department.Employees = employees.ToList();
        department.Managers = managers.ToList();
    }
}