using Application.Services;
using Domain.Entities;
using Domain.Interfaces;
using Moq;
using AutoMapper;
using FluentAssertions;

namespace Application.Tests.Services;

public class MenuServiceTests
{
    private readonly Mock<IMenuRepository> _mockMenuRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly MenuService _menuService;

    public MenuServiceTests()
    {
        _mockMenuRepository = new Mock<IMenuRepository>();
        _mockMapper = new Mock<IMapper>();

        _menuService = new MenuService(_mockMenuRepository.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task GetMenuTreeAsync_ShouldReturnHierarchicalMenuTree()
    {
        // Arrange
        var parentId = Guid.NewGuid();
        var child1Id = Guid.NewGuid();
        var child2Id = Guid.NewGuid();

        var menus = new List<Menu>
        {
            new() { Id = parentId, Name = "父菜单", ParentId = null, Sort = 1 },
            new() { Id = child1Id, Name = "子菜单1", ParentId = parentId, Sort = 1 },
            new() { Id = child2Id, Name = "子菜单2", ParentId = parentId, Sort = 2 }
        };

        _mockMenuRepository.Setup(r => r.GetMenuTreeAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(menus);

        // Act
        var result = await _menuService.GetMenuTreeAsync();

        // Assert
        result.Should().HaveCount(1);
        result[0].Id.Should().Be(parentId);
        result[0].Children.Should().HaveCount(2);
        result[0].Children[0].Name.Should().Be("子菜单1");
        result[0].Children[1].Name.Should().Be("子菜单2");
    }
}
