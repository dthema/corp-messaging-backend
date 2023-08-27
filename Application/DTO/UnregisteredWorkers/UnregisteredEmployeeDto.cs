using Domain.Models;

namespace Application.DTO.UnregisteredWorkers;

public record UnregisteredEmployeeDto(string Firstname, string Lastname, int DepartmentId) : UnregisteredWorkerDto(Firstname, Lastname, Role.Employee);