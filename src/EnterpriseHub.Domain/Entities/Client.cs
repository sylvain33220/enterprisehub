using EnterpriseHub.Domain.Common;

namespace EnterpriseHub.Domain.Entities;

public class Client : EntityBase
{
  public string Name {get; private set;} = default!;
  public string? Email {get; private set;}
  public string? Phone {get; private set;}
  public bool IsActive {get; private set;} = true;

#pragma warning disable CS8618
  protected Client() {}
#pragma warning restore CS8618
 public Client(string name, string? email, string? phone)
{
    SetName(name);
    SetEmail(email);
    SetPhone(phone);
    IsActive = true;
}

public void Update(string name, string? email, string? phone)
{
    SetName(name);
    SetEmail(email);
    SetPhone(phone);
    Touch();
}

public void Deactivate()
{
    IsActive = false;
    Touch();
}

public void Activate()
{
    IsActive = true;
    Touch();
}

private void SetName(string name)
{
    if (string.IsNullOrWhiteSpace(name))
        throw new ArgumentException("Client name is required.");

    Name = name.Trim();
}

private void SetEmail(string? email)
{
    Email = string.IsNullOrWhiteSpace(email) ? null : email.Trim().ToLowerInvariant();
}

private void SetPhone(string? phone)
{
    Phone = string.IsNullOrWhiteSpace(phone) ? null : phone.Trim();
}
}