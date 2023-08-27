using Application.DTO;
using Domain.Models;

namespace Application.Services.Interfaces;

public interface IAccountService
{
    Task<AccountDto> Create(int sessionId, AccountDto accountDto); 
    Task<AccountDto> Update(int sessionId, AccountDto accountDto);
    Task Delete(int sessionId, int workerId);
    Task<AccountDto> GetByWorkerId(int sessionId, int workerId);
    Task<AccountDto> GetByLogin(int sessionId, string login);
    Task<IEnumerable<AccountDto>> GetAll(int sessionId);
}