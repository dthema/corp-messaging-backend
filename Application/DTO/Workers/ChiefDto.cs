using Application.DTO.MessageSources;
using Domain.Models;

namespace Application.DTO.Workers;

public record ChiefDto(
    int Id,
    string Firstname,
    string Lastname,
    IReadOnlyCollection<MessageSourceDto> MessageSources)
    : WorkerDto(Id, Firstname, Lastname, Role.Chief, MessageSources);