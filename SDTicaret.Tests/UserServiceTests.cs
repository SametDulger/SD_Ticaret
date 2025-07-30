using SDTicaret.Application.Services;
using SDTicaret.Application.DTOs;
using SDTicaret.Core.Entities;
using Moq;
using AutoMapper;
using SDTicaret.Application.Interfaces;
using SDTicaret.Core.Interfaces;
using Xunit;

namespace SDTicaret.Tests
{
    public class UserServiceTests
    {
        [Fact]
        public async Task ActivateUserAsync_Should_Return_True_When_User_Exists()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var mapperMock = new Mock<IMapper>();
            
            var user = new User { Id = 1, Username = "testuser", IsActive = false };
            unitOfWorkMock.Setup(u => u.Repository<User>().GetByIdAsync(1)).ReturnsAsync(user);
            unitOfWorkMock.Setup(u => u.Repository<User>().Update(It.IsAny<User>())).Verifiable();
            unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
            
            var service = new UserService(unitOfWorkMock.Object, mapperMock.Object);
            
            // Act
            var result = await service.ActivateUserAsync(1);
            
            // Assert
            Assert.True(result);
            Assert.True(user.IsActive);
            unitOfWorkMock.Verify(u => u.Repository<User>().Update(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task ActivateUserAsync_Should_Return_False_When_User_Not_Exists()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var mapperMock = new Mock<IMapper>();
            
            unitOfWorkMock.Setup(u => u.Repository<User>().GetByIdAsync(999)).ReturnsAsync((User?)null);
            
            var service = new UserService(unitOfWorkMock.Object, mapperMock.Object);
            
            // Act
            var result = await service.ActivateUserAsync(999);
            
            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task DeactivateUserAsync_Should_Return_True_When_User_Exists()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var mapperMock = new Mock<IMapper>();
            
            var user = new User { Id = 1, Username = "testuser", IsActive = true };
            unitOfWorkMock.Setup(u => u.Repository<User>().GetByIdAsync(1)).ReturnsAsync(user);
            unitOfWorkMock.Setup(u => u.Repository<User>().Update(It.IsAny<User>())).Verifiable();
            unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
            
            var service = new UserService(unitOfWorkMock.Object, mapperMock.Object);
            
            // Act
            var result = await service.DeactivateUserAsync(1);
            
            // Assert
            Assert.True(result);
            Assert.False(user.IsActive);
            unitOfWorkMock.Verify(u => u.Repository<User>().Update(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task DeactivateUserAsync_Should_Return_False_When_User_Not_Exists()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var mapperMock = new Mock<IMapper>();
            
            unitOfWorkMock.Setup(u => u.Repository<User>().GetByIdAsync(999)).ReturnsAsync((User?)null);
            
            var service = new UserService(unitOfWorkMock.Object, mapperMock.Object);
            
            // Act
            var result = await service.DeactivateUserAsync(999);
            
            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task ChangeUserRoleAsync_Should_Return_True_When_User_Exists()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var mapperMock = new Mock<IMapper>();
            
            var user = new User { Id = 1, Username = "testuser", Role = "Customer" };
            unitOfWorkMock.Setup(u => u.Repository<User>().GetByIdAsync(1)).ReturnsAsync(user);
            unitOfWorkMock.Setup(u => u.Repository<User>().Update(It.IsAny<User>())).Verifiable();
            unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
            
            var service = new UserService(unitOfWorkMock.Object, mapperMock.Object);
            
            // Act
            var result = await service.ChangeUserRoleAsync(1, "Admin");
            
            // Assert
            Assert.True(result);
            Assert.Equal("Admin", user.Role);
            unitOfWorkMock.Verify(u => u.Repository<User>().Update(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task ChangeUserRoleAsync_Should_Return_False_When_User_Not_Exists()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var mapperMock = new Mock<IMapper>();
            
            unitOfWorkMock.Setup(u => u.Repository<User>().GetByIdAsync(999)).ReturnsAsync((User?)null);
            
            var service = new UserService(unitOfWorkMock.Object, mapperMock.Object);
            
            // Act
            var result = await service.ChangeUserRoleAsync(999, "Admin");
            
            // Assert
            Assert.False(result);
        }
    }
} 