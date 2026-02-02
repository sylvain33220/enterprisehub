using EnterpriseHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseHub.Infrastructure.Persistence;

public class EnterpriseHubDbContext : DbContext
{
  public EnterpriseHubDbContext(DbContextOptions<EnterpriseHubDbContext> options) : base(options)
  {}
  public DbSet<User> Users => Set<User>();
  public DbSet<Client> Clients => Set<Client>();
  public DbSet<Project> Projects => Set<Project>();
  public DbSet<Ticket> Tickets => Set<Ticket>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);
    modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();

    modelBuilder.Entity<Project>()
            .HasOne<Client>()
            .WithMany()
            .HasForeignKey(x => x.ClientId);

    modelBuilder.Entity<Ticket>()
            .HasOne<Project>()
            .WithMany()
            .HasForeignKey(x => x.ProjectId);
  }
}
