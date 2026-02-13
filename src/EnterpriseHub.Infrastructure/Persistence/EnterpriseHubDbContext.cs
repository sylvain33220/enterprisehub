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
using Microsoft.EntityFrameworkCore.Metadata;
using Npgsql.NameTranslation;

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
     // ✅ Force snake_case for Postgres (tables + columns)
    var translator = new NpgsqlSnakeCaseNameTranslator();

    foreach (var entity in modelBuilder.Model.GetEntityTypes())
    {
        // Table
        var tableName = entity.GetTableName();
        if (!string.IsNullOrWhiteSpace(tableName))
        {
            entity.SetTableName(translator.TranslateMemberName(tableName));
        }

        // Columns
        foreach (var property in entity.GetProperties())
        {
            var storeObjectId = StoreObjectIdentifier.Table(entity.GetTableName()!, entity.GetSchema());
            var columnName = property.GetColumnName(storeObjectId);

            if (!string.IsNullOrWhiteSpace(columnName))
            {
                property.SetColumnName(translator.TranslateMemberName(columnName));
            }
        }
    }
        // USERS
    modelBuilder.Entity<User>(b =>
    {
        b.ToTable("users");
        b.HasIndex(u => u.Email).IsUnique();
    });

    // CLIENTS
    modelBuilder.Entity<Client>(b =>
    {
        b.ToTable("clients");
        b.HasIndex(c => c.Email).IsUnique();
    });

    // PROJECTS
    modelBuilder.Entity<Project>(b =>
    {
        b.ToTable("projects");

        b.HasOne<Client>()
            .WithMany()
            .HasForeignKey(p => p.ClientId)
            .OnDelete(DeleteBehavior.Restrict); // optionnel mais souvent mieux
    });

    // TICKETS
    modelBuilder.Entity<Ticket>(b =>
    {
        b.ToTable("tickets");

        b.HasOne<Project>()
            .WithMany()
            .HasForeignKey(t => t.ProjectId)
            .OnDelete(DeleteBehavior.Restrict); // optionnel
    });
  }
}
