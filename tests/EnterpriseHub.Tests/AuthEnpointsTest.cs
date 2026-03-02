using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace EnterpriseHub.Tests.Api;

public class AuthEndpointsTests : IClassFixture<EnterpriseHubApiFactory>
{
    private readonly HttpClient _client;

    public AuthEndpointsTests(EnterpriseHubApiFactory factory)
    {
        _client = factory.CreateClient(new()
        {
            AllowAutoRedirect = false
        });
    }

    [Fact]
    public async Task Refresh_Should_Return_401_ProblemDetails_When_Missing_Cookie()
    {
        var res = await _client.PostAsync("/auth/refresh", content: null);
        var body = await res.Content.ReadAsStringAsync();
Console.WriteLine(res.StatusCode);
Console.WriteLine(res.Content.Headers.ContentType?.MediaType);
Console.WriteLine(body);

        res.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        res.Content.Headers.ContentType!.MediaType.Should().Be("application/problem+json");

        var problem = await res.Content.ReadFromJsonAsync<ProblemDetails>();
        problem!.Status.Should().Be(401);
        problem.Title.Should().NotBeNullOrWhiteSpace();
    }


    [Fact]
    public async Task Conflict_Should_Return_409_ProblemDetails()
    {
        var res = await _client.GetAsync("/__throw/conflict");
        res.StatusCode.Should().Be(HttpStatusCode.Conflict);

        var problem = await res.Content.ReadFromJsonAsync<ProblemDetails>();
        problem!.Status.Should().Be(409);
    }
    [Fact]
    public async Task Unknown_Route_Should_Return_404()
    {
        var res = await _client.GetAsync("/route/that/does/not/exist");

        res.StatusCode.Should().Be(HttpStatusCode.NotFound);
        
        // 404 ici c’est le pipeline ASP.NET, pas ton middleware (normal)
    }
    
}