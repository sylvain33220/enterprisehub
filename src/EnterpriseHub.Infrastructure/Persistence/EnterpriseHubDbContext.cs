/*
@author: Poteaux sylvain
@file: EnterpriseHubDbContext.cs
@description: DbContext implementation for the EnterpriseHub application, responsible for managing database access and providing DbSet properties for each entity. It also includes configuration for entity relationships and indexes, as well as implementations of methods defined in the IEnterpriseHubDbContext interface for user and refresh token management.
@version: 1.0
@date: 2026.03
@site: https://studio-purple.com
@mail : poteaux.sylvain@gmail.com
@license: MIT
*/
using EnterpriseHub.Application.Common.Interfaces;
using EnterpriseHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseHub.Infrastructure.Persistence;

public class EnterpriseHubDbContext : DbContext, IEnterpriseHubDbContext
{
    public EnterpriseHubDbContext(DbContextOptions<EnterpriseHubDbContext> options) : base(options) {}

    public DbSet<User> Users => Set<User>();
    public DbSet<Client> Clients => Set<Client>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<Ticket> Tickets => Set<Ticket>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(b =>
        {
            b.ToTable("users");
            b.HasIndex(u => u.Email).IsUnique();
        });

        modelBuilder.Entity<Client>(b =>
        {
            b.ToTable("clients");
            b.HasIndex(c => c.Email).IsUnique();
        });

        modelBuilder.Entity<Project>(b =>
        {
            b.ToTable("projects");

            b.HasOne<Client>()
                .WithMany()
                .HasForeignKey(p => p.ClientId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Ticket>(b =>
        {
            b.ToTable("tickets");

            b.HasOne<Project>()
                .WithMany()
                .HasForeignKey(t => t.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<RefreshToken>(b =>
        {
            b.ToTable("refresh_tokens");
            b.HasIndex(x => x.TokenHash).IsUnique();
            b.HasIndex(x => x.UserId);

            b.Property(x => x.TokenHash).HasMaxLength(2000).IsRequired();
            b.Property(x => x.CreatedByIp).HasMaxLength(64);
            b.Property(x => x.UserAgent).HasMaxLength(512);

            b.HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
      // ✅ Impl IEnterpriseHubDbContext
    public async Task<User?> FindUserByEmailAsync(string email, CancellationToken ct)
        => await Users.SingleOrDefaultAsync(u => u.Email == email, ct);

    public async Task<User?> GetUserByIdAsync(Guid userId, CancellationToken ct)
        => await Users.SingleOrDefaultAsync(u => u.Id == userId, ct);

    public async Task<RefreshToken?> FindRefreshTokenByHashAsync(string tokenHash, CancellationToken ct)
        => await RefreshTokens.SingleOrDefaultAsync(x => x.TokenHash == tokenHash, ct);

    public void AddRefreshToken(RefreshToken token) => RefreshTokens.Add(token);
}