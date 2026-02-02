using EnterpriseHub.Domain.Common;
using EnterpriseHub.Domain.Enums;

namespace EnterpriseHub.Domain.Entities;

public class Ticket : EntityBase
{
  public Guid ProjectId { get;private set; }
  public Guid? AssignedToUserId { get;private set; }

  public string Title { get;private set;} = default!;
  public string? Description { get;private set; }

  public TicketPriority Priority { get;private set;} = TicketPriority.Medium;
  public TicketStatus Status { get;private set;} = TicketStatus.Open;

  public DateTime? ResolvedAtUtc { get;private set; }
# pragma warning disable CS8618
   protected Ticket() {}
#pragma warning restore CS8618
  public Ticket(Guid projectId, string title, TicketPriority priority = TicketPriority.Medium)
  {
    if (projectId == Guid.Empty) throw new ArgumentException("ProjectId is required.");
    ProjectId = projectId;
    SetTitle(title);
    Priority = priority;
  }

  public void SetTitle(string title)
  {
    title = (title ?? "").Trim();
    if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Ticket title is required.");
    if (title.Length > 200) throw new ArgumentException("Ticket title is too long.");
    Title = title;
    Touch();
  }
  public void AssingnTo(Guid? userId)
  {
    if (userId.HasValue && userId == Guid.Empty)
      throw new ArgumentException("Invalid userId.");
    AssignedToUserId = userId;
    Touch();
  }
 public void ChangeStatus(TicketStatus status)
    {
        // règle métier : si Done -> ResolvedAt obligatoire
        if (status == TicketStatus.Done && ResolvedAtUtc is null)
            ResolvedAtUtc = DateTime.UtcNow;

        // si on repasse en Open/InProgress, on enlève ResolvedAt
        if ((status == TicketStatus.Open || status == TicketStatus.InProgress) && ResolvedAtUtc is not null)
            ResolvedAtUtc = null;

        Status = status;
        Touch();
    }

  public void ChangePriority(TicketPriority priority)
  {
    Priority = priority;
    Touch();
  }    
}