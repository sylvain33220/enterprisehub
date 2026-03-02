using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnterpriseHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRefreshTokens : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_projects_clients_client_id",
                table: "projects");

            migrationBuilder.DropForeignKey(
                name: "FK_tickets_projects_project_id",
                table: "tickets");

            migrationBuilder.RenameColumn(
                name: "role",
                table: "users",
                newName: "Role");

            migrationBuilder.RenameColumn(
                name: "email",
                table: "users",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "users",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "updated_at_utc",
                table: "users",
                newName: "UpdatedAtUtc");

            migrationBuilder.RenameColumn(
                name: "password_hash",
                table: "users",
                newName: "PasswordHash");

            migrationBuilder.RenameColumn(
                name: "last_name",
                table: "users",
                newName: "LastName");

            migrationBuilder.RenameColumn(
                name: "first_name",
                table: "users",
                newName: "FirstName");

            migrationBuilder.RenameColumn(
                name: "created_at_utc",
                table: "users",
                newName: "CreatedAtUtc");

            migrationBuilder.RenameIndex(
                name: "IX_users_email",
                table: "users",
                newName: "IX_users_Email");

            migrationBuilder.RenameColumn(
                name: "title",
                table: "tickets",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "status",
                table: "tickets",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "priority",
                table: "tickets",
                newName: "Priority");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "tickets",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "tickets",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "updated_at_utc",
                table: "tickets",
                newName: "UpdatedAtUtc");

            migrationBuilder.RenameColumn(
                name: "resolved_at_utc",
                table: "tickets",
                newName: "ResolvedAtUtc");

            migrationBuilder.RenameColumn(
                name: "project_id",
                table: "tickets",
                newName: "ProjectId");

            migrationBuilder.RenameColumn(
                name: "created_at_utc",
                table: "tickets",
                newName: "CreatedAtUtc");

            migrationBuilder.RenameColumn(
                name: "assigned_to_user_id",
                table: "tickets",
                newName: "AssignedToUserId");

            migrationBuilder.RenameIndex(
                name: "IX_tickets_project_id",
                table: "tickets",
                newName: "IX_tickets_ProjectId");

            migrationBuilder.RenameColumn(
                name: "status",
                table: "projects",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "projects",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "projects",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "budget",
                table: "projects",
                newName: "Budget");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "projects",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "updated_at_utc",
                table: "projects",
                newName: "UpdatedAtUtc");

            migrationBuilder.RenameColumn(
                name: "created_at_utc",
                table: "projects",
                newName: "CreatedAtUtc");

            migrationBuilder.RenameColumn(
                name: "client_id",
                table: "projects",
                newName: "ClientId");

            migrationBuilder.RenameIndex(
                name: "IX_projects_client_id",
                table: "projects",
                newName: "IX_projects_ClientId");

            migrationBuilder.RenameColumn(
                name: "phone",
                table: "clients",
                newName: "Phone");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "clients",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "email",
                table: "clients",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "clients",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "updated_at_utc",
                table: "clients",
                newName: "UpdatedAtUtc");

            migrationBuilder.RenameColumn(
                name: "is_active",
                table: "clients",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "created_at_utc",
                table: "clients",
                newName: "CreatedAtUtc");

            migrationBuilder.RenameIndex(
                name: "IX_clients_email",
                table: "clients",
                newName: "IX_clients_Email");

            migrationBuilder.CreateTable(
                name: "refresh_tokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    TokenHash = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RevokedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ReplacedByTokenHash = table.Column<string>(type: "text", nullable: true),
                    CreatedByIp = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    RevokedByIp = table.Column<string>(type: "text", nullable: true),
                    UserAgent = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_refresh_tokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_refresh_tokens_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_refresh_tokens_TokenHash",
                table: "refresh_tokens",
                column: "TokenHash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_refresh_tokens_UserId",
                table: "refresh_tokens",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_projects_clients_ClientId",
                table: "projects",
                column: "ClientId",
                principalTable: "clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_tickets_projects_ProjectId",
                table: "tickets",
                column: "ProjectId",
                principalTable: "projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_projects_clients_ClientId",
                table: "projects");

            migrationBuilder.DropForeignKey(
                name: "FK_tickets_projects_ProjectId",
                table: "tickets");

            migrationBuilder.DropTable(
                name: "refresh_tokens");

            migrationBuilder.RenameColumn(
                name: "Role",
                table: "users",
                newName: "role");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "users",
                newName: "email");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "users",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAtUtc",
                table: "users",
                newName: "updated_at_utc");

            migrationBuilder.RenameColumn(
                name: "PasswordHash",
                table: "users",
                newName: "password_hash");

            migrationBuilder.RenameColumn(
                name: "LastName",
                table: "users",
                newName: "last_name");

            migrationBuilder.RenameColumn(
                name: "FirstName",
                table: "users",
                newName: "first_name");

            migrationBuilder.RenameColumn(
                name: "CreatedAtUtc",
                table: "users",
                newName: "created_at_utc");

            migrationBuilder.RenameIndex(
                name: "IX_users_Email",
                table: "users",
                newName: "IX_users_email");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "tickets",
                newName: "title");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "tickets",
                newName: "status");

            migrationBuilder.RenameColumn(
                name: "Priority",
                table: "tickets",
                newName: "priority");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "tickets",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "tickets",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAtUtc",
                table: "tickets",
                newName: "updated_at_utc");

            migrationBuilder.RenameColumn(
                name: "ResolvedAtUtc",
                table: "tickets",
                newName: "resolved_at_utc");

            migrationBuilder.RenameColumn(
                name: "ProjectId",
                table: "tickets",
                newName: "project_id");

            migrationBuilder.RenameColumn(
                name: "CreatedAtUtc",
                table: "tickets",
                newName: "created_at_utc");

            migrationBuilder.RenameColumn(
                name: "AssignedToUserId",
                table: "tickets",
                newName: "assigned_to_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_tickets_ProjectId",
                table: "tickets",
                newName: "IX_tickets_project_id");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "projects",
                newName: "status");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "projects",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "projects",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "Budget",
                table: "projects",
                newName: "budget");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "projects",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAtUtc",
                table: "projects",
                newName: "updated_at_utc");

            migrationBuilder.RenameColumn(
                name: "CreatedAtUtc",
                table: "projects",
                newName: "created_at_utc");

            migrationBuilder.RenameColumn(
                name: "ClientId",
                table: "projects",
                newName: "client_id");

            migrationBuilder.RenameIndex(
                name: "IX_projects_ClientId",
                table: "projects",
                newName: "IX_projects_client_id");

            migrationBuilder.RenameColumn(
                name: "Phone",
                table: "clients",
                newName: "phone");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "clients",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "clients",
                newName: "email");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "clients",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAtUtc",
                table: "clients",
                newName: "updated_at_utc");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "clients",
                newName: "is_active");

            migrationBuilder.RenameColumn(
                name: "CreatedAtUtc",
                table: "clients",
                newName: "created_at_utc");

            migrationBuilder.RenameIndex(
                name: "IX_clients_Email",
                table: "clients",
                newName: "IX_clients_email");

            migrationBuilder.AddForeignKey(
                name: "FK_projects_clients_client_id",
                table: "projects",
                column: "client_id",
                principalTable: "clients",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tickets_projects_project_id",
                table: "tickets",
                column: "project_id",
                principalTable: "projects",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
