using Application.DTO.UnregisteredMessageSources;
using Domain.Models;

namespace Application.DTO.UnregisteredWorkers;

public abstract record UnregisteredWorkerDto(string Firstname, string Lastname, Role Role);