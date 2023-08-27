using Application.DTO;
using Application.DTO.Workers;
using Application.Services.Interfaces;
using Application.Utils.WorkerFactories;
using Domain.Exceptions;
using Domain.Models;
using Domain.Repositories;

namespace Application.Services;

public class DepartmentService : IDepartmentService
{
    private readonly IRepositoriesManager _repositoriesManager;
    private readonly IWorkerFactory _workerFactory;

    public DepartmentService(IRepositoriesManager repositoriesManager)
    {
        ArgumentNullException.ThrowIfNull(repositoriesManager);
        _repositoriesManager = repositoriesManager;
        _workerFactory = new EmployeeFactory();
        _workerFactory
            .AddNext(new ManagerFactory())
            .AddNext(new ChiefFactory());
    }

    public async Task<DepartmentDto> Create(int sessionId, string departmentName)
    {
        EmptyStringBadQueryException.ThrowIfStringNull(departmentName);

        using var transaction = await _repositoriesManager.DepartmentRepository.BeginTransactionAsync();

        var session = await _repositoriesManager.SessionRepository.GetSessionAsync(sessionId, transaction) ??
                      throw new FieldNotFoundException();
        var user = await _repositoriesManager.WorkerRepository.GetWorkerAsync(session.WorkerId, transaction) ??
                   throw new FieldNotFoundException();
        if (user.Role != Role.Chief)
            throw new BadQueryException("Only chiefs can manage departments");

        int departmentId = await _repositoriesManager.DepartmentRepository.InsertDepartmentAsync(new Department
        {
            Name = departmentName
        }, transaction) ?? throw new FieldNotFoundException();

        var department =
            await _repositoriesManager.DepartmentRepository.GetDepartmentAsync(departmentId, transaction) ??
            throw new FieldNotFoundException();

        transaction.Commit();
        transaction.Connection?.Close();

        return new DepartmentDto(
            department.Id,
            department.Name,
            department.Employees.Select(x => _workerFactory.CreateWorkerDto(x)).ToList(),
            department.Managers.Select(x => _workerFactory.CreateWorkerDto(x)).ToList());
    }

    public async Task<DepartmentDto> Update(int sessionId, RegisteredDepartmentDto departmentDto)
    {
        EmptyStringBadQueryException.ThrowIfStringNull(departmentDto.Name);

        using var transaction = await _repositoriesManager.DepartmentRepository.BeginTransactionAsync();

        var session = await _repositoriesManager.SessionRepository.GetSessionAsync(sessionId, transaction) ??
                      throw new FieldNotFoundException();
        var user = await _repositoriesManager.WorkerRepository.GetWorkerAsync(session.WorkerId, transaction) ??
                   throw new FieldNotFoundException();
        if (user.Role != Role.Chief)
            throw new BadQueryException("Only chiefs can manage departments");

        await _repositoriesManager.DepartmentRepository.InsertDepartmentAsync(new Department
        {
            Id = departmentDto.Id,
            Name = departmentDto.Name
        }, transaction);

        var department =
            await _repositoriesManager.DepartmentRepository.GetDepartmentAsync(departmentDto.Id, transaction) ??
            throw new FieldNotFoundException();

        transaction.Commit();
        transaction.Connection?.Close();

        return new DepartmentDto(
            department.Id,
            department.Name,
            department.Employees.Select(x => _workerFactory.CreateWorkerDto(x)).ToList(),
            department.Managers.Select(x => _workerFactory.CreateWorkerDto(x)).ToList());
    }

    public async Task Delete(int sessionId, int departmentId)
    {
        using var transaction = await _repositoriesManager.DepartmentRepository.BeginTransactionAsync();

        var session = await _repositoriesManager.SessionRepository.GetSessionAsync(sessionId, transaction) ??
                      throw new FieldNotFoundException();
        var user = await _repositoriesManager.WorkerRepository.GetWorkerAsync(session.WorkerId, transaction) ??
                   throw new FieldNotFoundException();
        if (user.Role != Role.Chief)
            throw new BadQueryException("Only chiefs can manage departments");

        await _repositoriesManager.DepartmentRepository.GetDepartmentAsync(departmentId, transaction);
        await _repositoriesManager.DepartmentRepository.DeleteDepartmentAsync(departmentId, transaction);

        transaction.Commit();
        transaction.Connection?.Close();
    }

    public async Task<DepartmentDto> GetById(int sessionId, int departmentId)
    {
        using var transaction = await _repositoriesManager.DepartmentRepository.BeginTransactionAsync();

        var session = await _repositoriesManager.SessionRepository.GetSessionAsync(sessionId, transaction) ??
                      throw new FieldNotFoundException();
        await _repositoriesManager.WorkerRepository.GetWorkerAsync(session.WorkerId, transaction);

        var department =
            await _repositoriesManager.DepartmentRepository.GetDepartmentAsync(departmentId, transaction) ??
            throw new FieldNotFoundException();

        transaction.Commit();
        transaction.Connection?.Close();

        return new DepartmentDto(
            department.Id,
            department.Name,
            department.Employees.Select(x => _workerFactory.CreateWorkerDto(x)).ToList(),
            department.Managers.Select(x => _workerFactory.CreateWorkerDto(x)).ToList());
    }

    public async Task<IEnumerable<DepartmentDto>> GetAll(int sessionId)
    {
        using var transaction = await _repositoriesManager.DepartmentRepository.BeginTransactionAsync();

        var session = await _repositoriesManager.SessionRepository.GetSessionAsync(sessionId, transaction) ??
                      throw new FieldNotFoundException();
        var user = await _repositoriesManager.WorkerRepository.GetWorkerAsync(session.WorkerId, transaction) ??
                   throw new FieldNotFoundException();
        if (user.Role != Role.Chief)
            throw new BadQueryException("Only chiefs can get all departments");

        var departments = await _repositoriesManager.DepartmentRepository.GetAllDepartmentsAsync(transaction);

        transaction.Commit();
        transaction.Connection?.Close();

        return departments.Select(department => new DepartmentDto(department.Id, department.Name,
                department.Employees.Select(x => _workerFactory.CreateWorkerDto(x)).ToList(),
                department.Managers.Select(x => _workerFactory.CreateWorkerDto(x)).ToList()))
            .ToList();
    }
}