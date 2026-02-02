using EnterpriseHub.Domain.Enums;
using EnterpriseHub.Domain.Common;

namespace EnterpriseHub.Domain.Entities;

public abstract class User : EntityBase
{
    public string Email { get; private set; } = default!;
    public string PasswordHash { get; private set; } = default!;
    public string FirstName { get; private set; } = default!;
    public string LastName { get; private set; } = default!;
    public UserRole Role { get; private set; } = UserRole.Dev;

    private User() { }

     public User(string email, string passwordHash, string firstName, string lastName, UserRole role = UserRole.Dev)
    {
        SetEmail(email);
        SetPasswordHash(passwordHash);
        SetName(firstName, lastName);
        Role = role;
    }

    public void SetEmail(string email)
      {
          email = (email ?? "").Trim().ToLowerInvariant();
          if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("Email is required.");
          if (email.Length > 320) throw new ArgumentException("Email is too long.");
          Email = email;
          Touch();
      }
    public void SetPasswordHash(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash)) throw new ArgumentException("Password hash is required.");
        PasswordHash = passwordHash;
        Touch();
    }

    public void SetName(string firstName, string lastName)
    {
        firstName = (firstName ?? "").Trim();
        lastName = (lastName ?? "").Trim();
        if (string.IsNullOrWhiteSpace(firstName)) throw new ArgumentException("First name is required.");
        if (string.IsNullOrWhiteSpace(lastName)) throw new ArgumentException("Last name is required.");
        if (firstName.Length > 100) throw new ArgumentException("First name is too long.");
        if (lastName.Length > 100) throw new ArgumentException("Last name is too long.");
        FirstName = firstName;
        LastName = lastName;
        Touch();
    }

    public void ChangeRole(UserRole role)
    {
        Role = role;
        Touch();
    }
}