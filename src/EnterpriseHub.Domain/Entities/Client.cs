using EnterpriseHub.Domain.Common;

namespace EnterpriseHub.Domain.Entities;

public class Client : EntityBase
{
  public string Name {get; private set;} = default!;
  public string? Email {get; private set;}
  public string? Phone {get; private set;}
  public bool IsActive {get; private set;} = true;

  private Client() {}

  public Client(string name , string? email = null, string? phone = null)
  {
    SetName(name);
    SetContact(email , phone);
  }

  public void SetName (string name)
  {
    name = (name ?? "").Trim();
    if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Client name is required.");
    if (name.Length > 100) throw new ArgumentException("Client name is too long.");
    Name = name;
    Touch();
  }

  public void SetContact(string? email, string? phone)
  {
    email = string.IsNullOrWhiteSpace(email) ? null : email.Trim().ToLowerInvariant();
    phone = string.IsNullOrWhiteSpace(phone) ? null : phone.Trim();

    Email = email;
    Phone = phone;
    Touch();
  }
  public void Disable()
  {
    IsActive = false;
    Touch();
  }
  public void Enable()
  {
    IsActive = true;
    Touch();
  }
}