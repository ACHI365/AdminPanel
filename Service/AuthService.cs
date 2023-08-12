using System.Text;
using AdminPanel.Data;
using AdminPanel.Model;
using AdminPanel.Model.Dto;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AdminPanel.Config;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AdminPanel.Service;

public class AuthService : IAuthService
{
    private readonly DataContext _context;
    private readonly JwtSettings _jwtSettings;

    public AuthService(DataContext context, IOptions<JwtSettings> jwtSettings)
    {
        _context = context;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<Result<User>> CreateUserAsync(UserDto userDto)
    {
        var existingUser = await _context.Users.SingleOrDefaultAsync(u => u.Email == userDto.Email);
        if (existingUser != null)
            return Result<User>.Fail("A user with the same email already exists.");
        User user = MapDto(userDto);
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return Result<User>.Success(user);
    }

    public async Task<Result<User?>> AuthenticateAsync(LogInDto logInDto)
    {
        var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == logInDto.Email);
        var valid = CheckValidity(user, logInDto.Password);
        if (valid != null) return valid;
        user!.LastLoginTime = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return Result<User?>.Success(user);
    }


    public string GenerateJwtToken(User user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512Signature);
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Name),
        };
        var token = new JwtSecurityToken(
            _jwtSettings.Issuer,
            _jwtSettings.Audience,
            claims,
            expires: DateTime.UtcNow.AddMinutes(int.Parse(_jwtSettings.ExpirationInMinutes)),
            signingCredentials: credentials
        );
        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.WriteToken(token);
    }

    private string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    private bool VerifyPassword(string password, string hashedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }

    private User MapDto(UserDto userDto)
    {
        var user = new User
        {
            Name = userDto.Name,
            PasswordHash = HashPassword(userDto.Password),
            Email = userDto.Email,
            LastLoginTime = DateTime.UtcNow,
            RegistrationTime = DateTime.UtcNow,
            IsBlocked = false
        };
        return user;
    }

    private Result<User?>? CheckValidity(User? user, string password)
    {
        if (user == null || !VerifyPassword(password, user.PasswordHash))
            return Result<User?>.Fail("User's mail or password is incorrect");
        if (user.IsBlocked)
            return Result<User?>.Fail("User is blocked.");
        return null;
    }
}