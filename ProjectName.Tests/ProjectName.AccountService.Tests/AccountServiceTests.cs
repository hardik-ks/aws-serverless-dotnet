using ProjectName.AccountService.Application.DTOs;
using ProjectName.AccountService.Application.Services;
using ProjectName.AccountService.Domain.Entities;
using ProjectName.AccountService.Repository.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace ProjectName.AccountService.Tests;

public class AccountServiceTests
{
    private readonly IAccountRepository _repository;
    private readonly ILogger<ProjectName.AccountService.Application.Services.AccountService> _logger;
    private readonly ProjectName.AccountService.Application.Services.AccountService _service;

    public AccountServiceTests()
    {
        _repository = Substitute.For<IAccountRepository>();
        _logger = Substitute.For<ILogger<ProjectName.AccountService.Application.Services.AccountService>>();
        _service = new ProjectName.AccountService.Application.Services.AccountService(_repository, _logger);
    }

    [Fact]
    public async Task GetByIdAsync_WhenEntityExists_ReturnsDto()
    {
        // Arrange
        var entity = new Account
        {
            Id = 1,
            Name = "Test Account",
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };
        _repository.GetByIdAsync(1).Returns(entity);

        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Name.Should().Be("Test Account");
    }

    //[Fact]
    //public async Task GetByIdAsync_WhenEntityNotFound_ReturnsNull()
    //{
    //    // Arrange
    //    _repository.GetByIdAsync(999).Returns((()null));

    //    // Act
    //    var result = await _service.GetByIdAsync(999);

    //    // Assert
    //    result.Should().BeNull();
    //}

    [Fact]
    public async Task GetAllAsync_ReturnsAllEntities()
    {
        // Arrange
        var entities = new List<Account>
        {
            new Account { Id = 1, Name = "Test 1", CreatedAt = DateTime.UtcNow, IsActive = true },
            new Account { Id = 2, Name = "Test 2", CreatedAt = DateTime.UtcNow, IsActive = true }
        };
        _repository.GetAllAsync().Returns(entities);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
    }
}
