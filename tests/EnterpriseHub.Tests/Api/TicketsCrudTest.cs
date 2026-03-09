/*
@author: Poteaux sylvain
@file: TicketsCrudTest.cs
@description: Integration tests for ticket CRUD operations, verifying creation, retrieval, and deletion of tickets.
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

public class TicketsCrudTest : IClassFixture<EnterpriseHubApiFactory>
{
    private readonly HttpClient _client;
    private readonly HttpClient _authClient;

    public TicketsCrudTest(EnterpriseHubApiFactory factory)
    {
        _client = factory.CreateClient();
        _authClient = factory.CreateAuthenticatedClient();
        
    }

    [Fact]
    public async Task Get_Ticket_Should_Return_404_When_NotFound()
    {
        var res = await _authClient.GetAsync("/api/v1.0/tickets/999999");

        var raw = await res.Content.ReadAsStringAsync();

        res.StatusCode.Should().Be(HttpStatusCode.NotFound);
        raw.Should().NotBeNull();
    }
    [Fact]
    public async Task Create_Then_Get_Then_Delete_Ticket_Should_Work()
    {
        // 1. Create client
        var clientPayload = new
        {
            name = "Client ticket test",
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
            name = "Projet ticket test",
            description = "Projet de test",
            clientId = createdClient.Id
        };
        var projectRes = await _authClient.PostAsJsonAsync("/api/v1.0/projects", projectPayload);
        var projectBody = await projectRes.Content.ReadAsStringAsync();
        projectRes.StatusCode.Should().Be(HttpStatusCode.Created, projectBody);
        var createdProject = await projectRes.Content.ReadFromJsonAsync<ProjectCreatedResponse>();
        createdProject.Should().NotBeNull();
        createdProject!.Id.Should().NotBe(Guid.Empty);
    
        // 3. Create ticket with real projectId
        var ticketPayload = new
        {
            title = "Ticket test",
            description = "Description du ticket de test",
            priority = (int)EnterpriseHub.Domain.Enums.TicketPriority.High,
            projectId = createdProject.Id
        };
        var ticketRes = await _authClient.PostAsJsonAsync("/api/v1.0/tickets", ticketPayload);
        var ticketBody = await ticketRes.Content.ReadAsStringAsync();
        ticketRes.StatusCode.Should().Be(HttpStatusCode.Created, ticketBody);
        var createdTicket = await ticketRes.Content.ReadFromJsonAsync<TicketCreatedResponse>();
        createdTicket.Should().NotBeNull();
        createdTicket!.Id.Should().NotBe(Guid.Empty);
        var ticketId = createdTicket.Id;
        // 4. Get
        var getRes = await _authClient.GetAsync($"/api/v1.0/tickets/{ticketId}");
        var getBody = await getRes.Content.ReadAsStringAsync(); 
        getRes.StatusCode.Should().Be(HttpStatusCode.OK, getBody);
        // 5. Delete
        var deleteRes = await _authClient.DeleteAsync($"/api/v1.0/tickets/{ticketId}");
        var deleteBody = await deleteRes.Content.ReadAsStringAsync(); 
        deleteRes.StatusCode.Should().Be(HttpStatusCode.NoContent, deleteBody);
        // 6. Verify deleted
        var getAfterDeleteRes = await _authClient.GetAsync($"/api/v1.0/tickets/{ticketId}");
        var getAfterDeleteBody = await getAfterDeleteRes.Content.ReadAsStringAsync();
        getAfterDeleteRes.StatusCode.Should().Be(HttpStatusCode.NotFound, getAfterDeleteBody);
    }
    private sealed class ClientCreatedResponse
    {
        public Guid Id { get; set; }
    }
    private sealed class ProjectCreatedResponse
    {
        public Guid Id { get; set; }
    }
    private sealed class TicketCreatedResponse
    {
        public Guid Id { get; set; }
    }
}