using System.Data;
using Dapper;
using Domain.Models;
using Npgsql;

namespace Domain.Repositories;

public interface IReportRepository : IBaseRepository
{
    Task<int?> InsertReportAsync(Report report, IDbTransaction? transaction = null);
    Task<int> DeleteReportAsync(int id, IDbTransaction? transaction = null);
    Task DeleteAllReportsAsync(IDbTransaction? transaction = null);
    Task<Report> GetReportAsync(int id, IDbTransaction? transaction = null);
    Task<Report?> GetReportByDepartmentIdAndDateAsync(int departmentId, DateOnly date, IDbTransaction? transaction = null);
    Task<IEnumerable<Report>> GetReportsByDateAsync(DateOnly date, IDbTransaction? transaction = null);
    Task<IEnumerable<Report>> GetReportsByDepartmentIdAsync(int departmentId, IDbTransaction? transaction = null);
    Task<IEnumerable<Report>> GetAllReportsAsync(IDbTransaction? transaction = null);
}