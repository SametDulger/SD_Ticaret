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
        var emailServiceMock = new Mock<IEmailService>();
        unitOfWorkMock.Setup(u => u.Repository<User>().GetAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<User, bool>>>())).ReturnsAsync(new User());
        var service = new AuthService(unitOfWorkMock.Object, mapperMock.Object, jwtServiceMock.Object, emailServiceMock.Object);
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
        var emailServiceMock = new Mock<IEmailService>();
        unitOfWorkMock.Setup(u => u.Repository<User>().GetAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<User, bool>>>())).ReturnsAsync((User?)null);
        unitOfWorkMock.Setup(u => u.Repository<User>().AddAsync(It.IsAny<User>())).Returns(Task.CompletedTask);
        unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
        mapperMock.Setup(m => m.Map<UserDto>(It.IsAny<User>())).Returns(new UserDto { Username = "test", Email = "test@test.com" });
        var service = new AuthService(unitOfWorkMock.Object, mapperMock.Object, jwtServiceMock.Object, emailServiceMock.Object);
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
        var emailServiceMock = new Mock<IEmailService>();
        unitOfWorkMock.Setup(u => u.Repository<User>().GetAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<User, bool>>>())).ReturnsAsync((User?)null);
        var service = new AuthService(unitOfWorkMock.Object, mapperMock.Object, jwtServiceMock.Object, emailServiceMock.Object);
        var loginDto = new LoginDto { Username = "nonexistent", Password = "123456" };
        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => service.LoginAsync(loginDto));
    }

    [Fact]
    public async Task ForgotPasswordAsync_Should_Return_True_When_User_Exists()
    {
        // Arrange
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var mapperMock = new Mock<IMapper>();
        var jwtServiceMock = new Mock<IJwtService>();
        var emailServiceMock = new Mock<IEmailService>();
        
        var user = new User { Id = 1, Email = "test@test.com", Username = "test" };
        unitOfWorkMock.Setup(u => u.Repository<User>().GetAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<User, bool>>>())).ReturnsAsync(user);
        unitOfWorkMock.Setup(u => u.Repository<User>().Update(It.IsAny<User>())).Verifiable();
        unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
        emailServiceMock.Setup(e => e.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);
        
        var service = new AuthService(unitOfWorkMock.Object, mapperMock.Object, jwtServiceMock.Object, emailServiceMock.Object);
        var forgotPasswordDto = new ForgotPasswordDto { Email = "test@test.com" };
        
        // Act
        var result = await service.ForgotPasswordAsync(forgotPasswordDto);
        
        // Assert
        Assert.True(result);
        unitOfWorkMock.Verify(u => u.Repository<User>().Update(It.IsAny<User>()), Times.Once);
        emailServiceMock.Verify(e => e.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task ForgotPasswordAsync_Should_Return_False_When_User_Not_Exists()
    {
        // Arrange
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var mapperMock = new Mock<IMapper>();
        var jwtServiceMock = new Mock<IJwtService>();
        var emailServiceMock = new Mock<IEmailService>();
        
        unitOfWorkMock.Setup(u => u.Repository<User>().GetAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<User, bool>>>())).ReturnsAsync((User?)null);
        
        var service = new AuthService(unitOfWorkMock.Object, mapperMock.Object, jwtServiceMock.Object, emailServiceMock.Object);
        var forgotPasswordDto = new ForgotPasswordDto { Email = "nonexistent@test.com" };
        
        // Act
        var result = await service.ForgotPasswordAsync(forgotPasswordDto);
        
        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ResetPasswordAsync_Should_Return_True_When_Valid_Token()
    {
        // Arrange
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var mapperMock = new Mock<IMapper>();
        var jwtServiceMock = new Mock<IJwtService>();
        var emailServiceMock = new Mock<IEmailService>();
        
        var token = Guid.NewGuid().ToString();
        var user = new User { Id = 1, Email = "test@test.com", RefreshToken = token, RefreshTokenExpiryTime = DateTime.UtcNow.AddHours(1) };
        unitOfWorkMock.Setup(u => u.Repository<User>().GetAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<User, bool>>>())).ReturnsAsync(user);
        unitOfWorkMock.Setup(u => u.Repository<User>().Update(It.IsAny<User>())).Verifiable();
        unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
        
        var service = new AuthService(unitOfWorkMock.Object, mapperMock.Object, jwtServiceMock.Object, emailServiceMock.Object);
        var resetPasswordDto = new ResetPasswordDto { Token = token, NewPassword = "newpassword123", ConfirmNewPassword = "newpassword123" };
        
        // Act
        var result = await service.ResetPasswordAsync(resetPasswordDto);
        
        // Assert
        Assert.True(result);
        unitOfWorkMock.Verify(u => u.Repository<User>().Update(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task ResetPasswordAsync_Should_Return_False_When_Invalid_Token()
    {
        // Arrange
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var mapperMock = new Mock<IMapper>();
        var jwtServiceMock = new Mock<IJwtService>();
        var emailServiceMock = new Mock<IEmailService>();
        
        unitOfWorkMock.Setup(u => u.Repository<User>().GetAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<User, bool>>>())).ReturnsAsync((User?)null);
        
        var service = new AuthService(unitOfWorkMock.Object, mapperMock.Object, jwtServiceMock.Object, emailServiceMock.Object);
        var resetPasswordDto = new ResetPasswordDto { Token = "invalid-token", NewPassword = "newpassword123", ConfirmNewPassword = "newpassword123" };
        
        // Act
        var result = await service.ResetPasswordAsync(resetPasswordDto);
        
        // Assert
        Assert.False(result);
    }
}