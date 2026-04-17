using Application.Services;
using Application.Interfaces;
using Application.DTOs;
using Domain.Entities;
using Domain.Interfaces;
using Moq;
using AutoMapper;
using FluentAssertions;

namespace Application.Tests.Services;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IJwtTokenProvider> _mockJwtTokenProvider;
    private readonly Mock<IMenuService> _mockMenuService;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockMapper = new Mock<IMapper>();
        _mockJwtTokenProvider = new Mock<IJwtTokenProvider>();
        _mockMenuService = new Mock<IMenuService>();

        _userService = new UserService(
            _mockUserRepository.Object,
            _mockMapper.Object,
            _mockJwtTokenProvider.Object,
            _mockMenuService.Object);
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ShouldReturnToken()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword("password123", 12);

        var user = new User
        {
            Id = userId,
            UserName = "testuser",
            Email = "test@example.com",
            PasswordHash = hashedPassword,
            IsActive = true,
            UserRoles = new List<UserRole>
            {
                new()
                {
                    Role = new Domain.Entities.Role { Name = "User" }
                }
            }
        };

        var loginDto = new Application.DTOs.LoginDto
        {
            UserName = "testuser",
            Password = "password123"
        };

        var userResponseDto = new Application.DTOs.UserResponseDto { Id = userId, UserName = "testuser" };
        var menuTree = new List<Application.DTOs.MenuTreeDto>();

        _mockUserRepository.Setup(r => r.FindByUserNameWithRolesAsync("testuser", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _mockJwtTokenProvider.Setup(j => j.GenerateToken(userId, "testuser", It.IsAny<List<string>>()))
            .Returns(("test_token", DateTime.UtcNow.AddHours(1)));
        _mockMapper.Setup(m => m.Map<Application.DTOs.UserResponseDto>(user)).Returns(userResponseDto);
        _mockMenuService.Setup(m => m.GetUserMenusAsync(userId, _mockUserRepository.Object, It.IsAny<CancellationToken>()))
            .ReturnsAsync(menuTree);

        // Act
        var result = await _userService.LoginAsync(loginDto);

        // Assert
        result.Token.Should().Be("test_token");
        result.User.UserName.Should().Be("testuser");
        result.Roles.Should().Contain("User");

        _mockJwtTokenProvider.Verify(j => j.GenerateToken(userId, "testuser", It.IsAny<List<string>>()), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_WithInvalidCredentials_ShouldThrowException()
    {
        // Arrange
        var loginDto = new Application.DTOs.LoginDto
        {
            UserName = "nonexistent",
            Password = "wrongpassword"
        };

        _mockUserRepository.Setup(r => r.FindByUserNameWithRolesAsync("nonexistent", It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act & Assert
        await Assert.ThrowsAsync<Common.Models.BusinessException>(
            () => _userService.LoginAsync(loginDto));
    }
}
