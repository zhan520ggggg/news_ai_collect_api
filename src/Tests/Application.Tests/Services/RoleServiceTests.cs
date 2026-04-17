using Application.DTOs;
using Application.Services;
using Domain.Entities;
using Domain.Interfaces;
using Moq;
using FluentAssertions;

namespace Application.Tests.Services;

public class RoleServiceTests
{
    private readonly Mock<IRepository<Role>> _mockRoleRepository;
    private readonly Mock<IRepository<RoleMenu>> _mockRoleMenuRepository;
    private readonly Mock<IMenuRepository> _mockMenuRepository;
    private readonly RoleService _roleService;

    public RoleServiceTests()
    {
        _mockRoleRepository = new Mock<IRepository<Role>>();
        _mockRoleMenuRepository = new Mock<IRepository<RoleMenu>>();
        _mockMenuRepository = new Mock<IMenuRepository>();

        _roleService = new RoleService(
            _mockRoleRepository.Object,
            _mockRoleMenuRepository.Object,
            _mockMenuRepository.Object);
    }

    [Fact]
    public async Task GetAllRolesWithMenusAsync_ShouldReturnRolesWithMenuIds()
    {
        // Arrange
        var roleId1 = Guid.NewGuid();
        var roleId2 = Guid.NewGuid();
        var menuId1 = Guid.NewGuid();
        var menuId2 = Guid.NewGuid();

        var roles = new List<Role>
        {
            new() { Id = roleId1, Name = "Admin", DisplayName = "管理员" },
            new() { Id = roleId2, Name = "User", DisplayName = "用户" }
        };

        var roleMenus = new List<RoleMenu>
        {
            new() { RoleId = roleId1, MenuId = menuId1 },
            new() { RoleId = roleId1, MenuId = menuId2 },
            new() { RoleId = roleId2, MenuId = menuId1 }
        };

        _mockRoleRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(roles);
        _mockRoleMenuRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(roleMenus);

        // Act
        var result = await _roleService.GetAllRolesWithMenusAsync();

        // Assert
        result.Should().HaveCount(2);
        result[0].RoleId.Should().Be(roleId1);
        result[0].MenuIds.Should().HaveCount(2);
        result[0].MenuIds.Should().ContainInOrder(new[] { menuId1, menuId2 });

        result[1].RoleId.Should().Be(roleId2);
        result[1].MenuIds.Should().HaveCount(1);
        result[1].MenuIds.Should().Contain(menuId1);
    }

    [Fact]
    public async Task CreateRoleAsync_ShouldCreateAndReturnRole()
    {
        // Arrange
        var roleId = Guid.NewGuid();
        var dto = new CreateRoleDto
        {
            Name = "NewRole",
            DisplayName = "新角色",
            Description = "测试角色"
        };

        _mockRoleRepository.Setup(r => r.AddAsync(It.IsAny<Role>(), It.IsAny<CancellationToken>()))
            .Callback<Role, CancellationToken>((role, _) => role.Id = roleId)
            .ReturnsAsync((Role role, CancellationToken _) => role);

        // Act
        var result = await _roleService.CreateRoleAsync(dto);

        // Assert
        result.Name.Should().Be("NewRole");
        result.DisplayName.Should().Be("新角色");
        result.Description.Should().Be("测试角色");

        _mockRoleRepository.Verify(r => r.AddAsync(
            It.Is<Role>(r => r.Name == "NewRole" && r.DisplayName == "新角色"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateRoleAsync_WithValidId_ShouldUpdateAndReturnRole()
    {
        // Arrange
        var roleId = Guid.NewGuid();
        var existingRole = new Role
        {
            Id = roleId,
            Name = "OldName",
            DisplayName = "旧名称",
            Description = "旧描述"
        };

        var dto = new UpdateRoleDto
        {
            Name = "NewName",
            DisplayName = "新名称"
        };

        _mockRoleRepository.Setup(r => r.GetByIdAsync(roleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingRole);
        _mockRoleRepository.Setup(r => r.UpdateAsync(It.IsAny<Role>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _roleService.UpdateRoleAsync(roleId, dto);

        // Assert
        result.Id.Should().Be(roleId);
        result.Name.Should().Be("NewName");
        result.DisplayName.Should().Be("新名称");
        result.Description.Should().Be("旧描述"); // Should remain unchanged

        _mockRoleRepository.Verify(r => r.UpdateAsync(existingRole, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteRoleAsync_WithValidId_ShouldDeleteRole()
    {
        // Arrange
        var roleId = Guid.NewGuid();
        var role = new Role
        {
            Id = roleId,
            Name = "TestRole",
            DisplayName = "测试角色"
        };

        _mockRoleRepository.Setup(r => r.GetByIdAsync(roleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(role);
        _mockRoleRepository.Setup(r => r.DeleteAsync(role, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _roleService.DeleteRoleAsync(roleId);

        // Assert
        _mockRoleRepository.Verify(r => r.DeleteAsync(role, It.IsAny<CancellationToken>()), Times.Once);
    }
}
