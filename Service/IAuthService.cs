using AdminPanel.Model;
using AdminPanel.Model.Dto;

namespace AdminPanel.Service;

public interface IAuthService
{
    Task<Result<User>> CreateUserAsync(UserDto userDto);
    Task<Result<User?>> AuthenticateAsync(LogInDto logInDto);
    string? GenerateJwtToken(User user);
}