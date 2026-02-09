/*
@file ProjectServiceTest.cs
@description Unit tests for the ProjectService in the EnterpriseHub application, using in-memory repositories to test project creation and update functionality.
@author Poteaux sylvain
@site https://www.studio-purple.com
@date 2026-09-02
@EnterpriseHub is licensed under the MIT License. See LICENSE file in the project root for full license information.
@version 1.0
*/

using EnterpriseHub.Application.Projects;
using EnterpriseHub.Application.Projects.Dto;
using EnterpriseHub.Application.Tests.Fakes;
using EnterpriseHub.Domain.Entities;
using FluentAssertions;
using Xunit;

public class ProjectServiceTests
{
    [Fact]
    public async Task CreateAsync_Should_Throw_When_Client_Not_Found()
    {
        var projects = new InMemoryProjectRepository();
        var clients = new InMemoryClientRepository();
        var svc = new ProjectService(projects, clients);

        Func<Task> act = () => svc.CreateAsync(new CreateProjectRequest(
            Name: "Website",
            ClientId: Guid.NewGuid(),
            Description: "desc",
            Budget: 1000m
        ), default);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("Client not found*");
    }

    [Fact]
    public async Task CreateAsync_Should_Create_Project_When_Client_Exists()
    {
        var projects = new InMemoryProjectRepository();
        var clients = new InMemoryClientRepository();

        var client = new Client("ACME", "contact@acme.fr", "0600000000");
        clients.Seed(client);

        var svc = new ProjectService(projects, clients);

        var created = await svc.CreateAsync(new CreateProjectRequest(
            Name: "ERP v1",
            ClientId: client.Id,
            Description: "First iteration",
            Budget: 5000m
        ), default);

        created.Id.Should().NotBeEmpty();
        created.Name.Should().Be("ERP v1");
        created.ClientId.Should().Be(client.Id);
        created.Description.Should().Be("First iteration");
    }

    [Fact]
    public async Task UpdateAsync_Should_Return_Null_When_Project_Not_Found()
    {
        var projects = new InMemoryProjectRepository();
        var clients = new InMemoryClientRepository();
        var svc = new ProjectService(projects, clients);

        var updated = await svc.UpdateAsync(Guid.NewGuid(), new UpdateProjectRequest(
            Name: "Updated",
            Description: "Updated desc"
        ), default);

        updated.Should().BeNull();
    }
}
