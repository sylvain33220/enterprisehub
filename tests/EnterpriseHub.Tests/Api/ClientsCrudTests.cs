/*
@author: Poteaux sylvain
@file: ClientsCrudTests.cs
@description: Integration tests for client CRUD operations, verifying creation, retrieval, and deletion of clients.
@version: 1.0
@date: 2026.03
@site: https://studio-purple.com
@mail : poteaux.sylvain@gmail.com
@license: MIT
*/
using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using EnterpriseHub.Tests.Api;

public class ClientsCrudTests : IClassFixture<EnterpriseHubApiFactory>
{
    private readonly HttpClient _client;
    private readonly HttpClient _authClient;

    public ClientsCrudTests(EnterpriseHubApiFactory factory)
    {
        _client = factory.CreateClient();
        _authClient = factory.CreateAuthenticatedClient();
        
    }

    [Fact]
    public async Task Get_Client_Should_Return_404_When_NotFound()
    {
        var res = await _authClient.GetAsync("/api/v1.0/clients/999999");

        var raw = await res.Content.ReadAsStringAsync();

        res.StatusCode.Should().Be(HttpStatusCode.NotFound);
        raw.Should().NotBeNull();
    }

   [Fact]
    public async Task Create_Then_Get_Then_Delete_Client_Should_Work()
    {
        var payload = new
        {
            name = "Client test",
            email = $"client{Guid.NewGuid():N}@test.com"
        };

        // Create
        var createRes = await _authClient.PostAsJsonAsync("/api/v1.0/clients", payload);
        var createBody = await createRes.Content.ReadAsStringAsync();

        createRes.StatusCode.Should().Be(HttpStatusCode.Created, createBody);

        var created = await createRes.Content.ReadFromJsonAsync<ClientCreatedResponse>();
        created.Should().NotBeNull();
        created!.Id.Should().NotBe(Guid.Empty);

        var clientId = created.Id;

        // Get
        var getRes = await _authClient.GetAsync($"/api/v1.0/clients/{clientId}");
        var getBody = await getRes.Content.ReadAsStringAsync();

        getRes.StatusCode.Should().Be(HttpStatusCode.OK, getBody);

        // Delete
        var deleteRes = await _authClient.DeleteAsync($"/api/v1.0/clients/{clientId}");
        var deleteBody = await deleteRes.Content.ReadAsStringAsync();

        deleteRes.StatusCode.Should().Be(HttpStatusCode.NoContent, deleteBody);

        // Verify deleted
        var getAfterDeleteRes = await _authClient.GetAsync($"/api/v1.0/clients/{clientId}");
        var getAfterDeleteBody = await getAfterDeleteRes.Content.ReadAsStringAsync();

        getAfterDeleteRes.StatusCode.Should().Be(HttpStatusCode.NotFound, getAfterDeleteBody);
    }

    private sealed class ClientCreatedResponse
    {
        public Guid Id { get; set; }
    }
}