/*
@file ClientServiceTest.cs
@description Unit tests for the ClientService in the EnterpriseHub application, using in-memory repositories to test client creation and deletion functionality.
@author Poteaux sylvain
@site https://www.studio-purple.com
@date 2026-09-02
@EnterpriseHub is licensed under the MIT License. See LICENSE file in the project root for full license information.
@version 1.0
*/
using EnterpriseHub.Application.Clients;
using EnterpriseHub.Application.Clients.Dto;
using EnterpriseHub.Application.Tests.Fakes;
using EnterpriseHub.Domain.Entities;
using FluentAssertions;
using Xunit;

public class ClientServiceTests
{
    [Fact]
    public async Task CreateAsync_Should_Create_Client()
    {
        // Arrange
        var repo = new InMemoryClientRepository();
        var svc = new ClientService(repo);

        // Act
        var created = await svc.CreateAsync(new CreateClientRequest(
            Name: "ACME Garage",
            Email: "contact@acme.fr",
            Phone: "0611223344"
        ));

        // Assert
        created.Id.Should().NotBeEmpty();
        created.Name.Should().Be("ACME Garage");
        created.Email.Should().Be("contact@acme.fr");
        created.Phone.Should().Be("0611223344");
    }

    [Fact]
    public async Task CreateAsync_Should_Throw_When_Email_Already_Exists()
    {
        // Arrange
        var repo = new InMemoryClientRepository();
        repo.Seed(new Client("Seed", "dup@acme.fr", null));

        var svc = new ClientService(repo);

        // Act
        Func<Task> act = () => svc.CreateAsync(new CreateClientRequest(
            Name: "New client",
            Email: "dup@acme.fr",
            Phone: null
        ));

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*email*exists*");
    }

    [Fact]
    public async Task DeleteAsync_Should_SoftDelete_Client_And_Not_Return_In_GetAll()
    {
        // Arrange
        var repo = new InMemoryClientRepository();
        var client = new Client("ACME", "soft@acme.fr", null);
        repo.Seed(client);

        var svc = new ClientService(repo);

        // Act
        var deleted = await svc.DeleteAsync(client.Id); 
        var all = await svc.GetAllAsync();

        // Assert
        deleted.Should().BeTrue();
        all.Should().NotContain(x => x.Id == client.Id);
    }
}
