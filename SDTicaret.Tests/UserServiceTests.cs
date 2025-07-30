using SDTicaret.Application.Services;
using SDTicaret.Application.DTOs;
using SDTicaret.Core.Entities;
using Moq;
using AutoMapper;
using SDTicaret.Application.Interfaces;
using SDTicaret.Core.Interfaces;
using Xunit;

public class UserServiceTests
{
    [Fact]
    public async Task GetAllAsync_Should_Return_All_Users()
    {
        // Arrange
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var mapperMock = new Mock<IMapper>();
        var users = new List<User>
        {
            new User { Id = 1, Username = "user1", Email = "user1@test.com" },
            new User { Id = 2, Username = "user2", Email = "user2@test.com" }
        };
        var userDtos = new List<UserDto>
        {
            new UserDto { Id = 1, Username = "user1", Email = "user1@test.com" },
            new UserDto { Id = 2, Username = "user2", Email = "user2@test.com" }
        };
        unitOfWorkMock.Setup(u => u.Repository<User>().GetAllAsync()).ReturnsAsync(users);
        mapperMock.Setup(m => m.Map<IEnumerable<UserDto>>(users)).Returns(userDtos);
        var service = new UserService(unitOfWorkMock.Object, mapperMock.Object);
        // Act
        var result = await service.GetAllAsync();
        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_User_When_Exists()
    {
        // Arrange
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var mapperMock = new Mock<IMapper>();
        var user = new User { Id = 1, Username = "test", Email = "test@test.com" };
        var userDto = new UserDto { Id = 1, Username = "test", Email = "test@test.com" };
        unitOfWorkMock.Setup(u => u.Repository<User>().GetByIdAsync(1)).ReturnsAsync(user);
        mapperMock.Setup(m => m.Map<UserDto>(user)).Returns(userDto);
        var service = new UserService(unitOfWorkMock.Object, mapperMock.Object);
        // Act
        var result = await service.GetByIdAsync(1);
        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("test", result.Username);
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Null_When_User_Not_Exists()
    {
        // Arrange
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var mapperMock = new Mock<IMapper>();
        unitOfWorkMock.Setup(u => u.Repository<User>().GetByIdAsync(999)).ReturnsAsync((User?)null);
        var service = new UserService(unitOfWorkMock.Object, mapperMock.Object);
        // Act
        var result = await service.GetByIdAsync(999);
        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AddAsync_Should_Add_User_Successfully()
    {
        // Arrange
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var mapperMock = new Mock<IMapper>();
        var userDto = new UserDto { Username = "newuser", Email = "newuser@test.com" };
        var user = new User { Username = "newuser", Email = "newuser@test.com" };
        mapperMock.Setup(m => m.Map<User>(userDto)).Returns(user);
        unitOfWorkMock.Setup(u => u.Repository<User>().AddAsync(user)).Returns(Task.CompletedTask);
        unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
        mapperMock.Setup(m => m.Map<UserDto>(user)).Returns(userDto);
        var service = new UserService(unitOfWorkMock.Object, mapperMock.Object);
        // Act
        var result = await service.AddAsync(userDto);
        // Assert
        Assert.NotNull(result);
        Assert.Equal("newuser", result.Username);
        unitOfWorkMock.Verify(u => u.Repository<User>().AddAsync(user), Times.Once);
        unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_Should_Update_User_Successfully()
    {
        // Arrange
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var mapperMock = new Mock<IMapper>();
        var userDto = new UserDto { Id = 1, Username = "updated", Email = "updated@test.com" };
        var existingUser = new User { Id = 1, Username = "old", Email = "old@test.com" };
        unitOfWorkMock.Setup(u => u.Repository<User>().GetByIdAsync(1)).ReturnsAsync(existingUser);
        unitOfWorkMock.Setup(u => u.Repository<User>().Update(existingUser));
        unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
        mapperMock.Setup(m => m.Map(userDto, existingUser));
        mapperMock.Setup(m => m.Map<UserDto>(existingUser)).Returns(userDto);
        var service = new UserService(unitOfWorkMock.Object, mapperMock.Object);
        // Act
        var result = await service.UpdateAsync(userDto);
        // Assert
        Assert.NotNull(result);
        Assert.Equal("updated", result.Username);
        unitOfWorkMock.Verify(u => u.Repository<User>().Update(existingUser), Times.Once);
        unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_Should_Throw_When_User_Not_Found()
    {
        // Arrange
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var mapperMock = new Mock<IMapper>();
        var userDto = new UserDto { Id = 999, Username = "nonexistent" };
        unitOfWorkMock.Setup(u => u.Repository<User>().GetByIdAsync(999)).ReturnsAsync((User?)null);
        var service = new UserService(unitOfWorkMock.Object, mapperMock.Object);
        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => service.UpdateAsync(userDto));
    }

    [Fact]
    public async Task DeleteAsync_Should_Delete_User_Successfully()
    {
        // Arrange
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var mapperMock = new Mock<IMapper>();
        var user = new User { Id = 1, Username = "todelete" };
        unitOfWorkMock.Setup(u => u.Repository<User>().GetByIdAsync(1)).ReturnsAsync(user);
        unitOfWorkMock.Setup(u => u.Repository<User>().Delete(user));
        unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
        var service = new UserService(unitOfWorkMock.Object, mapperMock.Object);
        // Act
        var result = await service.DeleteAsync(1);
        // Assert
        Assert.True(result);
        unitOfWorkMock.Verify(u => u.Repository<User>().Delete(user), Times.Once);
        unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_Should_Return_False_When_User_Not_Found()
    {
        // Arrange
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var mapperMock = new Mock<IMapper>();
        unitOfWorkMock.Setup(u => u.Repository<User>().GetByIdAsync(999)).ReturnsAsync((User?)null);
        var service = new UserService(unitOfWorkMock.Object, mapperMock.Object);
        // Act
        var result = await service.DeleteAsync(999);
        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ActivateUserAsync_Should_Activate_User_Successfully()
    {
        // Arrange
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var mapperMock = new Mock<IMapper>();
        var user = new User { Id = 1, Username = "test", IsActive = false };
        unitOfWorkMock.Setup(u => u.Repository<User>().GetByIdAsync(1)).ReturnsAsync(user);
        unitOfWorkMock.Setup(u => u.Repository<User>().Update(user));
        unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
        var service = new UserService(unitOfWorkMock.Object, mapperMock.Object);
        // Act
        var result = await service.ActivateUserAsync(1);
        // Assert
        Assert.True(result);
        Assert.True(user.IsActive);
        unitOfWorkMock.Verify(u => u.Repository<User>().Update(user), Times.Once);
        unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeactivateUserAsync_Should_Deactivate_User_Successfully()
    {
        // Arrange
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var mapperMock = new Mock<IMapper>();
        var user = new User { Id = 1, Username = "test", IsActive = true };
        unitOfWorkMock.Setup(u => u.Repository<User>().GetByIdAsync(1)).ReturnsAsync(user);
        unitOfWorkMock.Setup(u => u.Repository<User>().Update(user));
        unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
        var service = new UserService(unitOfWorkMock.Object, mapperMock.Object);
        // Act
        var result = await service.DeactivateUserAsync(1);
        // Assert
        Assert.True(result);
        Assert.False(user.IsActive);
        unitOfWorkMock.Verify(u => u.Repository<User>().Update(user), Times.Once);
        unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task GetByRoleAsync_Should_Return_Users_With_Specific_Role()
    {
        // Arrange
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var mapperMock = new Mock<IMapper>();
        var users = new List<User>
        {
            new User { Id = 1, Username = "admin1", Role = "Admin" },
            new User { Id = 2, Username = "admin2", Role = "Admin" }
        };
        var userDtos = new List<UserDto>
        {
            new UserDto { Id = 1, Username = "admin1", Role = "Admin" },
            new UserDto { Id = 2, Username = "admin2", Role = "Admin" }
        };
        unitOfWorkMock.Setup(u => u.Repository<User>().GetAllAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<User, bool>>>())).ReturnsAsync(users);
        mapperMock.Setup(m => m.Map<IEnumerable<UserDto>>(users)).Returns(userDtos);
        var service = new UserService(unitOfWorkMock.Object, mapperMock.Object);
        // Act
        var result = await service.GetByRoleAsync("Admin");
        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.All(result, user => Assert.Equal("Admin", user.Role));
    }
} 