using Application.Common.Models;
using Application.Users.Commands;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Moq;
using Test.Data;
using Test.Mocks;

namespace Application.UnitTest.Users.Commands;

public class UnlockUserCommandHandlerTests
{
    private readonly Mock<UserManager<User>> _mockUserManager;
    private readonly UnlockUserCommandHandler _handler;
    private readonly List<User> _users;

    public UnlockUserCommandHandlerTests()
    {
        _users = UserFaker.GenerateValidUsers();

        _mockUserManager = UserManagerMock.MockUserManager<User>();
        _mockUserManager.Setup(x => x.Users).Returns(DbContextMock.GetQueryableMockDbSet(_users));

        _handler = new UnlockUserCommandHandler(_mockUserManager.Object);
    }

    [Fact]
    public async Task HandleValidUser_ShouldReturnSuccess_WhenUserNotFound()
    {
        // Arrange
        User user = UserFaker.GenerateValidUser();

        UnlockUserCommand command = new() { Email = user.Email };

        _mockUserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);
        _mockUserManager.Setup(x => x.ResetAccessFailedCountAsync(user))
            .ReturnsAsync(IdentityResult.Success);
        _mockUserManager.Setup(x => x.SetLockoutEndDateAsync(It.IsAny<User>(), It.IsAny<DateTimeOffset>()))
            .ReturnsAsync(IdentityResult.Success);
        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.Succeeded);
        Assert.Contains("User account has been unlocked", result.Message);
    }


    [Fact]
    public async Task HandleInvalidUser_ShouldReturnFailure_WhenUserNotFound()
    {
        // Arrange
        User user = UserFaker.GenerateInvalidUser();

        UnlockUserCommand command = new() { Email = "UserNotFound@gmail.com" };

        _mockUserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((User)null);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Contains("User not found.", result.Message);
    }
    [Fact]
    public async Task HandleInvalidEmail_ShouldReturnFailure_WhenValidaionFails()
    {
        // Arrange
        User user = UserFaker.GenerateInvalidUser();
  
        UnlockUserCommand command = new() { Email = user.Email };

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Contains("Invalid email format", result.Message);
    }

    [Fact]
    public async Task HandleValidUser_ShouldReturnFailure_WhenResetAccessFailedCountFails()
    {
        // Arrange
        User user = UserFaker.GenerateValidUser();
        UnlockUserCommand command = new() { Email = user.Email };

        _mockUserManager.Setup(x => x.FindByEmailAsync(user.Email)).ReturnsAsync(user);

        _mockUserManager.Setup(x => x.ResetAccessFailedCountAsync(user))
            .ReturnsAsync(IdentityResult.Failed());

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Contains("Failed to reset access failed count.", result.Message);
    }

    [Fact]
    public async Task HandleValidUser_ShouldReturnFailure_WhenUnlockFails()
    {
        // Arrange
        User user = UserFaker.GenerateValidUser();
        UnlockUserCommand command = new() { Email = user.Email };

        _mockUserManager.Setup(x => x.FindByEmailAsync(user.Email)).ReturnsAsync(user);

        _mockUserManager.Setup(x => x.ResetAccessFailedCountAsync(user))
            .ReturnsAsync(IdentityResult.Success);
        _mockUserManager.Setup(x => x.SetLockoutEndDateAsync(user, It.IsAny<DateTimeOffset>()))
           .ReturnsAsync(IdentityResult.Failed());

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Contains("Failed to unlock the user.", result.Message);
    }

}

