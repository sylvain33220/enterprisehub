using EnterpriseHub.Application.Auth.Commands.Login;
using EnterpriseHub.Application.Auth.Commands.Refresh;
using EnterpriseHub.Application.Auth.Commands.Revoke;
using EnterpriseHub.Application.Common.Exceptions;
using EnterpriseHub.Application.Tests.Fakes;
using EnterpriseHub.Domain.Entities;
using EnterpriseHub.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace EnterpriseHub.Application.Tests.Auth;

public class AuthHandlersTests
{
    [Fact]
    public async Task Login_Should_Return_Tokens_And_Persist_RefreshToken()
    {
        var db = new InMemoryEnterpriseHubDbContext();
        var jwt = new FakeJwtTokenGenerator();
        var refresh = new FakeRefreshTokenService();
        var passwords = new FakePasswordHasher();

        var user = new User(
            email: "test@acme.fr",
            passwordHash: passwords.Hash("pwd"),
            firstName: "Test",
            lastName: "User",
            role: UserRole.Dev
        );

        db.SeedUser(user);

        var handler = new LoginHandler(db, jwt, refresh, passwords);

        var res = await handler.Handle(new LoginCommand
        {
            Email = "test@acme.fr",
            Password = "pwd",
            Ip = "127.0.0.1",
            UserAgent = "xunit"
        }, default);

        res.AccessToken.Should().StartWith("access-");
        res.RefreshToken.Should().StartWith("rt-");

        db.RefreshTokenStore.Should().HaveCount(1);
        db.RefreshTokenStore[0].UserId.Should().Be(user.Id);
        db.RefreshTokenStore[0].TokenHash.Should().Be($"hash-{res.RefreshToken}");
        db.RefreshTokenStore[0].IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task Refresh_Should_Rotate_And_Revoke_Old_Token()
    {
        var db = new InMemoryEnterpriseHubDbContext();
        var jwt = new FakeJwtTokenGenerator();
        var refresh = new FakeRefreshTokenService();
        var passwords = new FakePasswordHasher();

        var user = new User("test@acme.fr", passwords.Hash("pwd"), "Test", "User", UserRole.Dev);
        db.SeedUser(user);

        // login to seed first refresh
        var login = new LoginHandler(db, jwt, refresh, passwords);
        var loginRes = await login.Handle(new LoginCommand
        {
            Email = "test@acme.fr",
            Password = "pwd",
            Ip = "127.0.0.1",
            UserAgent = "xunit"
        }, default);

        var oldRefresh = loginRes.RefreshToken;
        var oldHash = $"hash-{oldRefresh}";

        var refreshHandler = new RefreshHandler(db, jwt, refresh);

        var refreshRes = await refreshHandler.Handle(new RefreshCommand
        {
            RefreshToken = oldRefresh,
            Ip = "127.0.0.1",
            UserAgent = "xunit"
        }, default);

        refreshRes.RefreshToken.Should().NotBe(oldRefresh);

        // old should be revoked
        var oldRt = db.RefreshTokenStore.Single(x => x.TokenHash == oldHash);
        oldRt.IsActive.Should().BeFalse();

        // new exists and active
        var newHash = $"hash-{refreshRes.RefreshToken}";
        db.RefreshTokenStore.Any(x => x.TokenHash == newHash).Should().BeTrue();
        db.RefreshTokenStore.Single(x => x.TokenHash == newHash).IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task Revoke_Should_Disable_RefreshToken()
    {
        var db = new InMemoryEnterpriseHubDbContext();
        var jwt = new FakeJwtTokenGenerator();
        var refresh = new FakeRefreshTokenService();
        var passwords = new FakePasswordHasher();

        var user = new User("test@acme.fr", passwords.Hash("pwd"), "Test", "User", UserRole.Dev);
        db.SeedUser(user);

        var login = new LoginHandler(db, jwt, refresh, passwords);
        var tokens = await login.Handle(new LoginCommand
        {
            Email = "test@acme.fr",
            Password = "pwd",
            Ip = "127.0.0.1",
            UserAgent = "xunit"
        }, default);

        var revoke = new RevokeHandler(db, refresh);
        await revoke.Handle(new RevokeCommand { RefreshToken = tokens.RefreshToken }, default);

        var stored = db.RefreshTokenStore.Single(x => x.TokenHash == $"hash-{tokens.RefreshToken}");
        stored.IsActive.Should().BeFalse();

        var refreshHandler = new RefreshHandler(db, jwt, refresh);
        var act = () => refreshHandler.Handle(new RefreshCommand
        {
            RefreshToken = tokens.RefreshToken,
            Ip = "127.0.0.1",
            UserAgent = "xunit"
        }, default);

        var ex = await act.Should().ThrowAsync<UnauthorizedException>();
        ex.Which.StatusCode.Should().Be(401);
    }
}