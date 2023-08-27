using Application.DTO.MessageSources;
using Application.DTO.UnregisteredWorkers;
using Domain.Models;

namespace Application.DTO.Workers;

public abstract record WorkerDto(
        int Id,
        string Firstname,
        string Lastname,
        Role Role,
        IReadOnlyCollection<MessageSourceDto> MessageSources)
    : UnregisteredWorkerDto(Firstname, Lastname, Role);