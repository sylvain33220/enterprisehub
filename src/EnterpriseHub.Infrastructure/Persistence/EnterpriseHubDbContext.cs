/*
@file EnterpriseHubDbContext.cs
@description Entity Framework Core DbContext for the EnterpriseHub application, managing database access for users, clients, projects, and tickets.
@author Poteaux sylvain
@site https://www.studio-purple.com
@mail poteaux.sylvain@gmail.com
@date 2026-09-02
@EnterpriseHub is licensed under the MIT License. See LICENSE file in the project root for full license information.
@version 1.0
*/
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

    modelBuilder.Entity<Client>()
    .HasIndex(c => c.Email)
    .IsUnique();

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
