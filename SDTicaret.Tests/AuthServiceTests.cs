using SDTicaret.Application.Services;
using SDTicaret.Application.DTOs;
using SDTicaret.Core.Entities;
using Moq;
using AutoMapper;
using SDTicaret.Application.Interfaces;
using SDTicaret.Core.Interfaces;
using Xunit;
using Microsoft.Extensions.Configuration;

public class AuthServiceTests
{
    [Fact]
    public async Task RegisterAsync_Should_Throw_When_Username_Exists()
    {
        // Arrange
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var mapperMock = new Mock<IMapper>();
        var jwtServiceMock = new Mock<IJwtService>();
        var configMock = new Mock<IConfiguration>();
        unitOfWorkMock.Setup(u => u.Repository<User>().GetAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<User, bool>>>())).ReturnsAsync(new User());
        var service = new AuthService(unitOfWorkMock.Object, mapperMock.Object, jwtServiceMock.Object, configMock.Object);
        var registerDto = new RegisterDto { Username = "test", Email = "test@test.com", Password = "123456", ConfirmPassword = "123456" };
        // Act & Assert
        await Assert.ThrowsAsync<System.InvalidOperationException>(() => service.RegisterAsync(registerDto));
    }

    [Fact]
    public async Task RegisterAsync_Should_Return_UserDto_When_Successful()
    {
        // Arrange
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var mapperMock = new Mock<IMapper>();
        var jwtServiceMock = new Mock<IJwtService>();
        var configMock = new Mock<IConfiguration>();
        unitOfWorkMock.Setup(u => u.Repository<User>().GetAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<User, bool>>>())).ReturnsAsync((User?)null);
        unitOfWorkMock.Setup(u => u.Repository<User>().AddAsync(It.IsAny<User>())).Returns(Task.CompletedTask);
        unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
        mapperMock.Setup(m => m.Map<UserDto>(It.IsAny<User>())).Returns(new UserDto { Username = "test", Email = "test@test.com" });
        var service = new AuthService(unitOfWorkMock.Object, mapperMock.Object, jwtServiceMock.Object, configMock.Object);
        var registerDto = new RegisterDto { Username = "test", Email = "test@test.com", Password = "123456", ConfirmPassword = "123456" };
        // Act
        var result = await service.RegisterAsync(registerDto);
        // Assert
        Assert.NotNull(result);
        Assert.Equal("test", result.Username);
        Assert.Equal("test@test.com", result.Email);
    }

    [Fact]
    public async Task LoginAsync_Should_Throw_When_User_Not_Found()
    {
        // Arrange
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var mapperMock = new Mock<IMapper>();
        var jwtServiceMock = new Mock<IJwtService>();
        var configMock = new Mock<IConfiguration>();
        unitOfWorkMock.Setup(u => u.Repository<User>().GetAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<User, bool>>>())).ReturnsAsync((User?)null);
        var service = new AuthService(unitOfWorkMock.Object, mapperMock.Object, jwtServiceMock.Object, configMock.Object);
        var loginDto = new LoginDto { Username = "nonexistent", Password = "123456" };
        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => service.LoginAsync(loginDto));
    }
}