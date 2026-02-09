/*
@file DatabaseServiceCollectionExtensions.cs
@description Extension methods for configuring database services in the EnterpriseHub application.
@author Poteaux sylvain
@site https://www.studio-purple.com
@mail poteaux.sylvain@gmail.com
@date 2026-09-02
@EnterpriseHub is licensed under the MIT License. See LICENSE file in the project root for full license information.
@version 1.0
*/
using EnterpriseHub.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EnterpriseHub.Infrastructure.Extensions;

public static class DatabaseServiceCollectionExtensions
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration config)
    {
        var provider = (config["Database:Provider"] ?? "postgres").Trim().ToLowerInvariant();
        var cs = config.GetConnectionString("Default");

        if (string.IsNullOrWhiteSpace(cs))
            throw new InvalidOperationException("Missing ConnectionStrings:Default");

        services.AddDbContext<EnterpriseHubDbContext>(options =>
        {
            switch (provider)
            {
                case "postgres":
                case "postgresql":
                case "npgsql":
                    options.UseNpgsql(cs);
                    break;

                case "mysql":
                case "mariadb":
                     options.UseMySql(cs, ServerVersion.AutoDetect(cs));
                break;
                
                default:
                    throw new NotSupportedException($"Unsupported database provider '{provider}'.");
            }
        });

        return services;
    }
}
