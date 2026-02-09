using EnterpriseHub.Domain.Enums;
using EnterpriseHub.Domain.Common;

namespace EnterpriseHub.Domain.Entities;

public class Project : EntityBase
{
   public Guid ClientId {get;private set;}
   public string Name {get;private set;} = default!;
   public string? Description {get;private set;}
   public ProjectStatus Status { get; private set;} = ProjectStatus.Draft;

   public decimal? Budget {get;private set;}

#pragma warning disable CS8618
   protected Project(Guid clientId)
  {
    ClientId = clientId;
  }
#pragma warning restore CS8618

  public Project(Guid clientId, string name, string? description = null , DateOnly? startDate = null, DateOnly? endDate = null, decimal? budget = null)
  {
    if (clientId == Guid.Empty) throw new ArgumentException("ClientId is required.");
    ClientId = clientId;
    SetName(name);
    Description = description;
  }

  public void SetName(string name)
  {
    name = (name ?? "").Trim();
    if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Project name is required.");
    if (name.Length > 200) throw new ArgumentException("Project name is too long.");
    Name = name;
    Touch();
  }

  public void SetBudget(decimal? budget)
  {
    if (budget.HasValue && budget.Value < 0) throw new ArgumentException("Budget must be >= 0.");
    Budget = budget;
    Touch();
  }

  public void ChangeStatus(ProjectStatus status)
  {
    Status = status;
    Touch();
  }

    public void Update(string name, string? description)
    {
        SetName(name);
        Description = description?.Trim();
        Touch();
    }

}