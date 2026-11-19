using ProjectName.AdminService.Application.DTOs;
using ProjectName.AdminService.Application.Services;
using ProjectName.AdminService.Domain.Entities;
using ProjectName.AdminService.Repository.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace ProjectName.AdminService.Tests;

public class AdminServiceTests
{
    private readonly IAdminRepository _repository;
    private readonly ILogger<ProjectName.AdminService.Application.Services.AdminService> _logger;
    private readonly ProjectName.AdminService.Application.Services.AdminService _service;

    public AdminServiceTests()
    {
        _repository = Substitute.For<IAdminRepository>();
        _logger = Substitute.For<ILogger<ProjectName.AdminService.Application.Services.AdminService>>();
        _service = new ProjectName.AdminService.Application.Services.AdminService(_repository, _logger);
    }

    [Fact]
    public async Task GetByIdAsync_WhenEntityExists_ReturnsDto()
    {
        // Arrange
        var entity = new Admin
        {
            Id = 1,
            Name = "Test Admin",
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };
        _repository.GetByIdAsync(1).Returns(entity);

        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Name.Should().Be("Test Admin");
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
        var entities = new List<Admin>
        {
            new Admin { Id = 1, Name = "Test 1", CreatedAt = DateTime.UtcNow, IsActive = true },
            new Admin { Id = 2, Name = "Test 2", CreatedAt = DateTime.UtcNow, IsActive = true }
        };
        _repository.GetAllAsync().Returns(entities);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
    }
}
