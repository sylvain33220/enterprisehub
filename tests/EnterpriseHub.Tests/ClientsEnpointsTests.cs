using System.Net;
using System.Net.Http.Json;
using EnterpriseHub.Tests.Api;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

public class ClientsEndpointsTests : IClassFixture<EnterpriseHubApiFactory>
{
    private readonly HttpClient _client;
    private readonly HttpClient _authClient;

    public ClientsEndpointsTests(EnterpriseHubApiFactory factory)
    {
        _client = factory.CreateClient();
        _authClient = factory.CreateAuthenticatedClient();
    }

    [Fact]
    public async Task Get_Client_Should_Return_401_When_NotAuthenticated()
    {
        var id = Guid.NewGuid();

        var res = await _client.GetAsync($"/api/v1.0/clients/{id}");

        res.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Get_Client_Should_Return_404_When_NotFound_And_Authenticated()
    {
        var id = Guid.NewGuid();

        var res = await _authClient.GetAsync($"/api/v1.0/clients/{id}");

        res.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var problem = await res.Content.ReadFromJsonAsync<ProblemDetails>();

        problem.Should().NotBeNull();
        problem!.Status.Should().Be(404);
        problem.Title.Should().Be("Not Found");
        problem.Detail.Should().Contain("Client");
        problem.Extensions.Should().ContainKey("traceId");
    }
}