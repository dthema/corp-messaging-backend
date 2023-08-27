using System.Data;
using Dapper;
using Domain.Models;
using Domain.Repositories;
using Npgsql;

namespace Infrastructure.Repositories;

public class ReportRepository : BaseRepository, IReportRepository
{
    public ReportRepository()
    {
        SimpleCRUD.SetDialect(SimpleCRUD.Dialect.PostgreSQL);
    }
    
    public async Task<int?> InsertReportAsync(Report report, IDbTransaction? transaction = null) =>
        await InsertAsync(report, transaction);

    public async Task<int> DeleteReportAsync(int id, IDbTransaction? transaction = null) =>
        await DeleteAsync<Report>(id, transaction);

    public async Task DeleteAllReportsAsync(IDbTransaction? transaction = null) =>
        await DeleteAllAsync<Report>(transaction);
    
    public async Task<Report> GetReportAsync(int id, IDbTransaction? transaction = null)
    {
        var report = await GetAsync<Report>(id, transaction);
        await FillReport(report);
        return report;
    }

    public async Task<Report?> GetReportByDepartmentIdAndDateAsync(int departmentId, DateOnly date, IDbTransaction? transaction = null)
    {
        await using var connection = new NpgsqlConnection(ConnectionString);
        var reports = await connection.GetListAsync<Report>(
            "WHERE \"Date\" = @Date AND \"DepartmentId\" = @Id",
            new { Date = date, Id = departmentId }, transaction) ?? new List<Report>();
        var reportsList = reports.ToList();
        await FillReports(reportsList);
        return reportsList.FirstOrDefault(report => report.DepartmentId == departmentId);
    }

    public async Task<IEnumerable<Report>> GetReportsByDateAsync(DateOnly date, IDbTransaction? transaction = null)
    {
        await using var connection = new NpgsqlConnection(ConnectionString);
        var reports = await connection.GetListAsync<Report>(
            "WHERE \"Date\" = @Date",
            new { Date = date }, transaction) ?? new List<Report>();
        var reportsList = reports.ToList();
        await FillReports(reportsList);
        return reportsList;
    }

    public async Task<IEnumerable<Report>> GetReportsByDepartmentIdAsync(int departmentId, IDbTransaction? transaction = null)
    {
        await using var connection = new NpgsqlConnection(ConnectionString);
        var reports = await connection.GetListAsync<Report>(
            "WHERE \"DepartmentId\" = @Id",
            new { Id = departmentId }, transaction) ?? new List<Report>();
        var reportsList = reports.ToList();
        await FillReports(reportsList);
        return reportsList;
    }

    public async Task<IEnumerable<Report>> GetAllReportsAsync(IDbTransaction? transaction = null)
    {
        await using var connection = new NpgsqlConnection(ConnectionString);
        var reports = await GetListAsync<Report>();
        var reportsList = reports.ToList();
        await FillReports(reportsList);
        return reportsList;
    }

    private async Task FillReport(Report report)
    {
        var sourceAnalyticsRepository = new SourceAnalyticsRepository();
        IEnumerable<SourceAnalytics> sourcesAnalytics = new List<SourceAnalytics>();
        if (report.DepartmentId == 0)
            sourcesAnalytics = await sourceAnalyticsRepository.GetSourcesAnalyticsByDateAsync(report.Date);
        else
            sourcesAnalytics = await sourceAnalyticsRepository.GetSourcesAnalyticsByDepartmentIdAndDateAsync(report.DepartmentId, report.Date);
        report.SourcesAnalytics = sourcesAnalytics.ToList();
    }
    
    private async Task FillReports(IReadOnlyCollection<Report> reports)
    {
        foreach (Report report in reports)
            await FillReport(report);
    }
}