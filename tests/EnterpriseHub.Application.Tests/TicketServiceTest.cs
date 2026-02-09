/*
 * File: TicketService.cs
 * Description: Application service responsible for ticket management.
 *
 * Author: Sylvain Poteaux
 * Website: https://www.studio-purple.com
 *
 * © 2026 EnterpriseHub
 * Licensed under the MIT License.
 */

using EnterpriseHub.Application.Tickets;
using EnterpriseHub.Application.Tickets.Dto;
using EnterpriseHub.Application.Tests.Fakes;
using EnterpriseHub.Domain.Entities;
using EnterpriseHub.Domain.Enums;
using FluentAssertions;
using Xunit;
using System.ComponentModel;

public class TicketServiceTests
{
  [Fact]
  public async Task CreateAsync_Should_Throw_When_Project_Not_Found()
  {
    var tickets = new InMemoryTicketRepository();
    var projects = new InMemoryProjectRepository();
    var svc = new TicketService(tickets, projects);

    Func<Task> act = () => svc.CreateAsync(new CreateTicketRequest(
      Title: "Bug login",
      ProjectId: Guid.NewGuid(),
      Description: "Cannot login",
      Priority: TicketPriority.Medium
    ), default);
    await act.Should().ThrowAsync<KeyNotFoundException>()
      .WithMessage("Project not found*");
  }

  [Fact]
  public async Task CreateAsync_Should_Create_Ticket_When_Project_Exists()
  {
    var tickets = new InMemoryTicketRepository();
    var projects = new InMemoryProjectRepository();

    var project = new Project(Guid.NewGuid(), "P1", null);
    projects.Seed(project);

    var svc = new TicketService(tickets, projects);

    var created = await svc.CreateAsync(new CreateTicketRequest(
      Title: "Bug login",
      ProjectId: project.Id,
      Description: "Cannot login",
      Priority: TicketPriority.High
    ), default);

    created.Id.Should().NotBeEmpty();
    created.ProjectId.Should().Be(project.Id);
    created.Priority.Should().Be(TicketPriority.High);
    created.Status.Should().Be(TicketStatus.Open);
  }
}