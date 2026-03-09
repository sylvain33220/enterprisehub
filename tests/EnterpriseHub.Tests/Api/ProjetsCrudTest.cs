/*
@author: Poteaux sylvain
@file: ProjetsCrudTest.cs
@description: Integration tests for project CRUD operations, verifying creation, retrieval, and deletion of projects.
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

public class ProjetsCrudTest : IClassFixture<EnterpriseHubApiFactory>
{
    private readonly HttpClient _client;
    private readonly HttpClient _authClient;

    public ProjetsCrudTest(EnterpriseHubApiFactory factory)
    {
        _client = factory.CreateClient();
        _authClient = factory.CreateAuthenticatedClient();
        
    }

    [Fact]
    public async Task Get_Projet_Should_Return_404_When_NotFound()
    {
        var res = await _authClient.GetAsync("/api/v1.0/projects/999999");

        var raw = await res.Content.ReadAsStringAsync();

        res.StatusCode.Should().Be(HttpStatusCode.NotFound);
        raw.Should().NotBeNull();
    }

   [Fact]
public async Task Create_Then_Get_Then_Delete_Project_Should_Work()
{
    // 1. Create client
    var clientPayload = new
    {
        name = "Client projet test",
        email = $"client{Guid.NewGuid():N}@test.com"
    };

    var clientRes = await _authClient.PostAsJsonAsync("/api/v1.0/clients", clientPayload);
    var clientBody = await clientRes.Content.ReadAsStringAsync();

    clientRes.StatusCode.Should().Be(HttpStatusCode.Created, clientBody);

    var createdClient = await clientRes.Content.ReadFromJsonAsync<ClientCreatedResponse>();
    createdClient.Should().NotBeNull();
    createdClient!.Id.Should().NotBe(Guid.Empty);

    // 2. Create project with real clientId
    var projectPayload = new
    {
        name = "Projet test",
        description = "Projet de test",
        clientId = createdClient.Id
    };

    var createRes = await _authClient.PostAsJsonAsync("/api/v1.0/projects", projectPayload);
    var createBody = await createRes.Content.ReadAsStringAsync();

    createRes.StatusCode.Should().Be(HttpStatusCode.Created, createBody);

    var createdProject = await createRes.Content.ReadFromJsonAsync<ProjectCreatedResponse>();
    createdProject.Should().NotBeNull();
    createdProject!.Id.Should().NotBe(Guid.Empty);

    var projectId = createdProject.Id;

    // 3. Get
    var getRes = await _authClient.GetAsync($"/api/v1.0/projects/{projectId}");
    var getBody = await getRes.Content.ReadAsStringAsync();

    getRes.StatusCode.Should().Be(HttpStatusCode.OK, getBody);

    // 4. Delete
    var deleteRes = await _authClient.DeleteAsync($"/api/v1.0/projects/{projectId}");
    var deleteBody = await deleteRes.Content.ReadAsStringAsync();

    deleteRes.StatusCode.Should().Be(HttpStatusCode.NoContent, deleteBody);

    // 5. Verify deleted
    var getAfterDeleteRes = await _authClient.GetAsync($"/api/v1.0/projects/{projectId}");
    var getAfterDeleteBody = await getAfterDeleteRes.Content.ReadAsStringAsync();

    getAfterDeleteRes.StatusCode.Should().Be(HttpStatusCode.NotFound, getAfterDeleteBody);
}
[Fact] 
public async Task Create_Project_Should_Fail_When_Client_Not_Exist()
{
    var projectPayload = new
    {
        name = "Projet test",
        description = "Projet de test",
        clientId = Guid.NewGuid() // ClientId aléatoire qui n'existe pas
    };

    var createRes = await _authClient.PostAsJsonAsync("/api/v1.0/projects", projectPayload);
    var createBody = await createRes.Content.ReadAsStringAsync();

    createRes.StatusCode.Should().Be(HttpStatusCode.BadRequest, createBody);
}
[Fact]
public async Task Update_Project_Should_Work()
{
    // 1. Create client
    var clientPayload = new
    {
        name = "Client projet test",
        email = $"client{Guid.NewGuid():N}@test.com"

    };
    var clientRes = await _authClient.PostAsJsonAsync("/api/v1.0/clients", clientPayload);
    var clientBody = await clientRes.Content.ReadAsStringAsync();
    clientRes.StatusCode.Should().Be(HttpStatusCode.Created, clientBody);
    var createdClient = await clientRes.Content.ReadFromJsonAsync<ClientCreatedResponse>();
    createdClient.Should().NotBeNull();
    createdClient!.Id.Should().NotBe(Guid.Empty);
    // 2. Create project with real clientId
    var projectPayload = new
    {
        name = "Projet test",
        description = "Projet de test",
        clientId = createdClient.Id
    };
    var createRes = await _authClient.PostAsJsonAsync("/api/v1.0/projects", projectPayload);
    var createBody = await createRes.Content.ReadAsStringAsync();
    createRes.StatusCode.Should().Be(HttpStatusCode.Created, createBody);
    var createdProject = await createRes.Content.ReadFromJsonAsync<ProjectCreatedResponse>();
    createdProject.Should().NotBeNull();
    createdProject!.Id.Should().NotBe(Guid.Empty);
    var projectId = createdProject.Id;
    // 3. Update
    var updatePayload = new
    {
        name = "Projet test updated",
        description = "Projet de test mis à jour"
    };
    var updateRes = await _authClient.PutAsJsonAsync($"/api/v1.0/projects/{projectId}", updatePayload);
    var updateBody = await updateRes.Content.ReadAsStringAsync();
    updateRes.StatusCode.Should().Be(HttpStatusCode.OK, updateBody);
    // 4. Get updated project
    var getRes = await _authClient.GetAsync($"/api/v1.0/projects/{projectId}");
    var getBody = await getRes.Content.ReadAsStringAsync();
    getRes.StatusCode.Should().Be(HttpStatusCode.OK, getBody);
    var updatedProject = await getRes.Content.ReadFromJsonAsync<ProjectCreatedResponse>();
    updatedProject.Should().NotBeNull();
    updatedProject!.Id.Should().Be(projectId);
}
private sealed class ClientCreatedResponse
{
    public Guid Id { get; set; }
}

private sealed class ProjectCreatedResponse
{
    public Guid Id { get; set; }
}
}