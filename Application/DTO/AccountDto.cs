using System.Runtime.CompilerServices;

namespace Application.DTO;

public record AccountDto(string Login, string Password, int WorkerId) : UnregisteredAccountDto(Login, Password);