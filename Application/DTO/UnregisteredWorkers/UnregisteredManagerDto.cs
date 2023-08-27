using Domain.Models;

namespace Application.DTO.UnregisteredWorkers;

public record UnregisteredManagerDto(string Firstname, string Lastname, int DepartmentId) : UnregisteredWorkerDto(Firstname, Lastname, Role.Manager);