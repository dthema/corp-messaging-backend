using Application.DTO.Workers;

namespace Application.DTO;

public record DepartmentDto(int Id, string Name, IReadOnlyCollection<WorkerDto> Employees, IReadOnlyCollection<WorkerDto> Managers)
    : RegisteredDepartmentDto(Id, Name);