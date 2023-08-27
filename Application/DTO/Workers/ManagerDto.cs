using Application.DTO.MessageSources;
using Domain.Models;

namespace Application.DTO.Workers;

public record ManagerDto(
    int Id,
    string Firstname,
    string Lastname,
    int DepartmentId,
    IReadOnlyCollection<MessageSourceDto> MessageSources)
    : WorkerDto(Id, Firstname, Lastname, Role.Manager, MessageSources);