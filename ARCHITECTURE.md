EnterpriseHub — Architecture Blueprint
1. Objectif

Cette architecture vise à garder une API :

lisible

testable

maintenable

découplée de la technique

cohérente dans les patterns utilisés

Le projet repose sur 4 couches principales :

Api

Application

Domain

Infrastructure

2. Structure des projets
src/
  EnterpriseHub.Api/
  EnterpriseHub.Application/
  EnterpriseHub.Domain/
  EnterpriseHub.Infrastructure/

tests/
  EnterpriseHub.UnitTests/
  EnterpriseHub.IntegrationTests/
3. Rôle de chaque couche
EnterpriseHub.Api

Responsable de l’exposition HTTP.

Contient notamment :

controllers ou endpoints

configuration DI

middleware

auth

swagger

configuration de l’application

mapping HTTP ↔ Application

Cette couche :

reçoit les requêtes

appelle MediatR

retourne une réponse HTTP

Elle ne doit pas contenir de logique métier.

EnterpriseHub.Application

Responsable des cas d’usage.

Contient notamment :

Commands

Queries

Handlers

Validators

DTOs

interfaces

exceptions applicatives

behaviors MediatR

Cette couche :

orchestre les actions métier

applique les règles applicatives

dépend du domaine

s’appuie sur des abstractions

EnterpriseHub.Domain

Responsable du métier pur.

Contient notamment :

entités

value objects

enums

règles métier

exceptions métier éventuelles

Cette couche :

ne connaît ni HTTP

ni EF Core

ni Dapper

ni MediatR

ni ASP.NET Core

C’est la couche la plus stable et la plus pure.

EnterpriseHub.Infrastructure

Responsable des détails techniques.

Contient notamment :

EF Core

Dapper

DbContext

repositories

services JWT

hashing

date/time providers

accès base de données

migrations

intégrations externes

Cette couche implémente les interfaces définies dans Application.

4. Règles de dépendances
Dépendances autorisées
Api -> Application
Api -> Infrastructure   (uniquement pour le bootstrap / DI)

Application -> Domain

Infrastructure -> Application
Infrastructure -> Domain

Domain -> rien
Dépendances interdites
Domain ne doit jamais dépendre de :

Application

Infrastructure

Api

EF Core

ASP.NET Core

MediatR

Application ne doit jamais dépendre de :

Api

implémentations concrètes d’Infrastructure

Api ne doit jamais :

parler directement à la base

contenir de logique métier

exécuter du SQL

manipuler EF Core pour les use cases

5. Organisation recommandée dans Application
EnterpriseHub.Application/
  Common/
    Behaviors/
    Exceptions/
    Interfaces/
    Models/

  Features/
    Auth/
      Commands/
        Login/
          LoginCommand.cs
          LoginCommandHandler.cs
          LoginCommandValidator.cs
          LoginResponse.cs

        RefreshToken/
          RefreshTokenCommand.cs
          RefreshTokenCommandHandler.cs
          RefreshTokenResponse.cs

      Queries/
        Me/
          MeQuery.cs
          MeQueryHandler.cs
          MeResponse.cs

Cette organisation permet :

de regrouper chaque use case

d’éviter les gros dossiers fourre-tout

de garder une lecture claire par feature

6. Pattern MediatR à stabiliser
Command

Utiliser un Command lorsqu’on modifie l’état du système :

création

modification

suppression

action métier

Exemples :

LoginCommand

CreateUserCommand

DeleteProjectCommand

Un Command retourne généralement :

Unit

un identifiant

un DTO de résultat

Query

Utiliser une Query lorsqu’on lit des données sans effet de bord.

Exemples :

GetCurrentUserQuery

GetProjectByIdQuery

GetProjectsQuery

Une Query retourne un DTO de lecture.

Règles

Un handler = un use case

Un handler ne construit pas de réponse HTTP

Un handler ne dépend pas de HttpContext

Un handler ne contient pas de SQL inline

Un handler reste focalisé sur l’orchestration métier

7. Exemple minimal de flow
Request HTTP
public sealed record LoginRequest(string Email, string Password);
Controller
[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request, CancellationToken ct)
    {
        var result = await _mediator.Send(
            new LoginCommand(request.Email, request.Password),
            ct
        );

        return Ok(result);
    }
}
Command
public sealed record LoginCommand(string Email, string Password) : IRequest<LoginResponse>;
Response DTO
public sealed record LoginResponse(
    string AccessToken,
    DateTime ExpiresAtUtc
);
Handler
public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly IUserRepository _users;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtService _jwtService;

    public LoginCommandHandler(
        IUserRepository users,
        IPasswordHasher passwordHasher,
        IJwtService jwtService)
    {
        _users = users;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
    }

    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken ct)
    {
        var user = await _users.GetByEmailAsync(request.Email, ct);

        if (user is null)
            throw new UnauthorizedAppException("Invalid credentials.");

        var isValid = _passwordHasher.Verify(request.Password, user.PasswordHash);

        if (!isValid)
            throw new UnauthorizedAppException("Invalid credentials.");

        var token = _jwtService.GenerateAccessToken(user);

        return new LoginResponse(token.Token, token.ExpiresAtUtc);
    }
}
8. Validation

La validation se découpe en 2 niveaux.

Validation applicative

Dans Application.

Exemples :

champ requis

format email

longueur minimum

pagination valide

ID positif

Exemple avec FluentValidation :

public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8);
    }
}
Validation métier

Dans Domain.

Exemples :

transition d’état interdite

date incohérente

règle métier fondamentale

invariants d’entité

La couche Domain protège la cohérence métier.

9. Exceptions applicatives

Toutes les erreurs métiers/applicatives remontées aux endpoints doivent converger vers une base commune.

Base
public abstract class AppException : Exception
{
    public int StatusCode { get; }
    public string Title { get; }
    public string? ErrorCode { get; }

    protected AppException(string title, string message, int statusCode, string? errorCode = null)
        : base(message)
    {
        Title = title;
        StatusCode = statusCode;
        ErrorCode = errorCode;
    }
}
Exceptions spécialisées
public sealed class NotFoundAppException : AppException
{
    public NotFoundAppException(string message)
        : base("Not Found", message, StatusCodes.Status404NotFound, "not_found")
    {
    }
}

public sealed class UnauthorizedAppException : AppException
{
    public UnauthorizedAppException(string message)
        : base("Unauthorized", message, StatusCodes.Status401Unauthorized, "unauthorized")
    {
    }
}

public sealed class ForbiddenAppException : AppException
{
    public ForbiddenAppException(string message)
        : base("Forbidden", message, StatusCodes.Status403Forbidden, "forbidden")
    {
    }
}

public sealed class ConflictAppException : AppException
{
    public ConflictAppException(string message)
        : base("Conflict", message, StatusCodes.Status409Conflict, "conflict")
    {
    }
}
Validation exception
public sealed class ValidationAppException : AppException
{
    public IDictionary<string, string[]> Errors { get; }

    public ValidationAppException(IDictionary<string, string[]> errors)
        : base(
            "Validation failed",
            "One or more validation errors occurred.",
            StatusCodes.Status400BadRequest,
            "validation_error")
    {
        Errors = errors;
    }
}
10. Middleware global d’exception

La couche Api doit traduire toute AppException en ProblemDetails.

Règle

Les handlers lèvent des exceptions applicatives

Le middleware transforme cela en réponse HTTP homogène

Les controllers restent minces

Exemple simplifié
public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationAppException ex)
        {
            await WriteProblemDetails(context, ex.StatusCode, ex.Title, ex.Message, ex.ErrorCode, ex.Errors);
        }
        catch (AppException ex)
        {
            await WriteProblemDetails(context, ex.StatusCode, ex.Title, ex.Message, ex.ErrorCode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");

            await WriteProblemDetails(
                context,
                StatusCodes.Status500InternalServerError,
                "Server Error",
                "An unexpected error occurred.",
                "server_error");
        }
    }

    private static async Task WriteProblemDetails(
        HttpContext context,
        int status,
        string title,
        string detail,
        string? errorCode,
        object? errors = null)
    {
        context.Response.StatusCode = status;
        context.Response.ContentType = "application/problem+json";

        var problem = new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = detail,
            Type = $"https://httpstatuses.com/{status}"
        };

        if (!string.IsNullOrWhiteSpace(errorCode))
            problem.Extensions["code"] = errorCode;

        if (errors is not null)
            problem.Extensions["errors"] = errors;

        await context.Response.WriteAsJsonAsync(problem);
    }
}
11. EF Core et Dapper

Le projet peut suivre une stratégie simple et pragmatique.

EF Core

À privilégier pour :

créations

modifications

suppressions

agrégats métier

transactions

persistance du write side

Dapper

À privilégier pour :

lecture

projections

listes paginées

endpoints read-only

requêtes optimisées

Convention recommandée

Commands → plutôt EF Core

Queries → plutôt Dapper

Cela donne un CQRS léger, lisible et réaliste.

12. Interfaces et implémentations

Les interfaces vivent dans Application.
Les implémentations vivent dans Infrastructure.

Exemple
public interface IJwtService
{
    AccessTokenResult GenerateAccessToken(User user);
}
public sealed class JwtService : IJwtService
{
    public AccessTokenResult GenerateAccessToken(User user)
    {
        // implémentation technique
    }
}

Même principe pour :

IPasswordHasher

IDateTimeProvider

IUserRepository

IRefreshTokenRepository

IUserReadService

13. Tests
Tests unitaires

Ils vérifient :

handlers

services purs

règles métier

comportements isolés

Caractéristiques :

rapides

sans HTTP

sans vraie base

avec mocks/fakes si nécessaire

Exemples :

LoginCommandHandler_Should_ReturnToken_When_CredentialsAreValid

LoginCommandHandler_Should_ThrowUnauthorized_When_PasswordIsInvalid

Tests d’intégration

Ils vérifient :

pipeline HTTP complet

routing

middleware

auth

ProblemDetails

sérialisation

persistance réelle ou environnement de test

Exemples :

POST /api/auth/login returns 200

POST /api/auth/refresh without cookie returns 401

GET /__throw/conflict returns 409 problem details

Répartition
À tester en unitaire

Application

Domain

À tester en intégration

Api

Infrastructure branchée au runtime

14. Conventions de code
Controllers

fins

sans logique métier

traduisent HTTP vers MediatR

Handlers

un use case chacun

orchestrent

ne retournent pas IActionResult

DTOs

dédiés à l’entrée/sortie

pas d’exposition brute des entités de domaine

Exceptions

toujours dériver de AppException pour les erreurs connues

Réponses d’erreur

toujours via ProblemDetails

15. Behaviors MediatR recommandés

À stabiliser progressivement :

ValidationBehavior

LoggingBehavior

PerformanceBehavior plus tard

TransactionBehavior si nécessaire sur certains commands

Ordre recommandé

validation

logging

handler

16. Exemple de ValidationBehavior
public sealed class ValidationBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
            return await next();

        var context = new ValidationContext<TRequest>(request);

        var results = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = results
            .SelectMany(r => r.Errors)
            .Where(f => f is not null)
            .GroupBy(f => f.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(x => x.ErrorMessage).Distinct().ToArray());

        if (failures.Count != 0)
            throw new ValidationAppException(failures);

        return await next();
    }
}
17. Enregistrement DI recommandé
Application
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }
}
Infrastructure
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
        {
            // config provider
        });

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();

        return services;
    }
}
18. Anti-patterns à éviter

logique métier dans les controllers

DbContext injecté directement dans Api

SQL dans les handlers

services “fourre-tout” type AuthService qui fait tout

mélange lecture/écriture dans un même handler

exceptions génériques partout

réponses HTTP construites dans Application

entités de domaine exposées directement au front

19. Doctrine de projet

La doctrine retenue pour EnterpriseHub est la suivante :

Domain = métier pur

Application = use cases et orchestration

Infrastructure = détails techniques

Api = transport HTTP

Et :

toute action = Command

toute lecture = Query

toute erreur connue = AppException

toute erreur HTTP = ProblemDetails

toute logique métier testable hors HTTP

toute route testable en intégration

20. Checklist de review d’une feature

Avant de valider une feature, vérifier :

le controller est-il fin ?

le use case est-il exprimé via Command ou Query ?

la logique métier est-elle dans le handler ou le domaine ?

la validation est-elle gérée proprement ?

les erreurs passent-elles via AppException ?

les réponses HTTP sont-elles homogènes ?

y a-t-il au moins un test pertinent ?

la lecture et l’écriture sont-elles bien séparées ?

21. Cible de maturité

L’objectif n’est pas d’avoir une architecture “académique”, mais une architecture :

claire

constante

pragmatique

facile à faire évoluer

On préfère :

une convention simple respectée partout
à

une architecture ultra-complexe appliquée à moitié