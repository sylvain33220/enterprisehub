/*
 * File: AuthService.cs
 * Description: Application service responsible for ticket management.
 *
 * Author: Sylvain Poteaux
 * Website: https://www.studio-purple.com
 *
 * © 2026 EnterpriseHub
 * Licensed under the MIT License.
 */

using EnterpriseHub.Application.Auth.Dto;
using EnterpriseHub.Application.Auth.Ports;
using EnterpriseHub.Domain.Entities;
using EnterpriseHub.Domain.Enums;

namespace EnterpriseHub.Application.Auth;

public class AuthService
{
  private readonly IUserRepository _users;
     private readonly IPasswordHasher _hasher;
    private readonly IJwtTokenGenerator _jwt;


public AuthService(IUserRepository users, IPasswordHasher hasher, IJwtTokenGenerator jwt)
{
    _users = users;
    _hasher = hasher;
    _jwt = jwt;
}

public async Task<AuthResponse> RegisterAsync(RegisterRequest req , CancellationToken ct =default)
  {
    var email = (req.Email ?? "").Trim().ToLowerInvariant();
    if(string.IsNullOrWhiteSpace(email)) throw new ArgumentException("Email is required", nameof(req.Email));
    if(string.IsNullOrWhiteSpace(req.Password) || req.Password.Length < 8) throw new ArgumentException("Password is required", nameof(req.Password));

    var existing = await _users.GetUserByEmailAsync(email,ct);
    if(existing is not null) throw new InvalidOperationException("User with this email already exists");

    var hash = _hasher.Hash(req.Password);

    var user = new User(
      email:email,
      passwordHash:hash,
      firstName:req.FirstName,
      lastName:req.LastName,
      role:UserRole.Dev
    );

    await _users.AddAsync(user,ct);

    var token = _jwt.GenerateToken(user);
    return new AuthResponse(token,
        new
        {
            user.Id,
            user.Email,
            user.FirstName,
            user.LastName,
            Role = user.Role.ToString()
        });
  }

  public async Task<AuthResponse> LoginAsync(LoginRequest req , CancellationToken ct = default)
  {
     var email = (req.Email ?? "").Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("Email is required.");
        if (string.IsNullOrWhiteSpace(req.Password)) throw new ArgumentException("Password is required.");

        var user = await _users.GetUserByEmailAsync(email, ct);
        if (user is null) throw new UnauthorizedAccessException("Invalid credentials.");

        var ok = _hasher.Verify(req.Password, user.PasswordHash);
        if (!ok) throw new UnauthorizedAccessException("Invalid credentials.");

        var token = _jwt.GenerateToken(user);
        return new AuthResponse(token,
            new
            {
                user.Id,
                user.Email,
                user.FirstName,
                user.LastName,
                Role = user.Role.ToString()
            });
  }
    public async Task<AuthResponse> LogoutAsync(LoginRequest req , CancellationToken ct = default)
  {
      // For JWT, logout is typically handled on the client side by deleting the token.
      // Optionally, you can implement token blacklisting on the server side if needed.
      await Task.CompletedTask;
      return new AuthResponse(string.Empty, new { });
  }

}