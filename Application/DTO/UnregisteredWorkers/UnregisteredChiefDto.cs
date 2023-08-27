using Domain.Models;

namespace Application.DTO.UnregisteredWorkers;

public record UnregisteredChiefDto(string Firstname, string Lastname) : UnregisteredWorkerDto(Firstname, Lastname, Role.Chief);