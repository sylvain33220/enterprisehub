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
using EnterpriseHub.Application.Common.Exceptions;
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
    if(string.IsNullOrWhiteSpace(email)) throw new UnauthorizedAppException("Email is required");
    if(string.IsNullOrWhiteSpace(req.Password) || req.Password.Length < 8) throw new UnauthorizedAppException("Password is required");

    var existing = await _users.GetUserByEmailAsync(email,ct);
    if(existing is not null) throw new ConflictAppException($"User with this email {email} already exists");

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
        if (string.IsNullOrWhiteSpace(email)) throw new UnauthorizedAppException("Email is required.");
        if (string.IsNullOrWhiteSpace(req.Password)) throw new UnauthorizedAppException("Password is required.");

        var user = await _users.GetUserByEmailAsync(email, ct);
        if (user is null) throw new UnauthorizedAppException("Invalid credentials.");

        var ok = _hasher.Verify(req.Password, user.PasswordHash);
        if (!ok) throw new UnauthorizedAppException("Invalid credentials.");

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