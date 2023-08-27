using Application.DTO;
using Application.DTO.UnregisteredWorkers;

namespace Application.Services.Interfaces;

public interface IInitializationService
{
    Task<ChiefAccountDto> CreateChiefWithAccount(UnregisteredChiefAccountDto chiefAccountDto);
}