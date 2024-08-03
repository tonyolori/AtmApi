using Application.Common.Models;
using Application.Users.Commands;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using Test.Data;
using Test.Mocks;

namespace Test.Users.Commands;

public class DeleteUserCommandTest
{
    private readonly List<User> _users;
    private readonly Mock<UserManager<User>> _mockUserManager;


    public DeleteUserCommandTest()
    {
        _users = UserFaker.GenerateValidUsers();

        _mockUserManager = UserManagerMock.MockUserManager<User>();

        DbSet<User> DbUsers = DbContextMock.GetQueryableMockDbSet(_users);
        _mockUserManager.Setup(x => x.Users).Returns(DbUsers);

    }

    [Fact]
    public async Task Handle_ValidAccountNumber_ReturnsSuccess()
    {
        // Arrange
        User user = UserFaker.GenerateValidUser();

        DeleteUserCommandHandler handler = new(_mockUserManager.Object);


        // Act
        Result result = await handler.Handle(new DeleteUserCommand { AccountNumber = user.AccountNumber }, CancellationToken.None);

        // Assert
        Assert.True(result.Succeeded);
        Assert.Contains("User Deleted", result.Message);
    }

    [Fact]
    public async Task Handle_ValidEmail_ReturnsSuccess()
    {
        // Arrange
        User user = UserFaker.GenerateValidUser();
        _mockUserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
           .ReturnsAsync(user);

        DeleteUserCommandHandler handler = new(_mockUserManager.Object);

        // Act
        Result result = await handler.Handle(new DeleteUserCommand { Email = user.Email }, CancellationToken.None);

        // Assert
        Assert.True(result.Succeeded);
        Assert.Contains("User Deleted", result.Message);
    }

    [Fact]
    public async Task Handle_InvalidEmail_ReturnsFailure()
    {
        // Arrange
        User user = UserFaker.GenerateInvalidUser();
        _mockUserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
          .ReturnsAsync((User?)null);

        DeleteUserCommandHandler handler = new(_mockUserManager.Object);

        // Act
        Result result = await handler.Handle(new DeleteUserCommand { Email = user.Email }, CancellationToken.None);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Contains("Invalid User", result.Message);
    }


    [Fact]
    public async Task Handle_InvalidAccountNumber_ReturnsFailure()
    {
        // Arrange
        User user = UserFaker.GenerateInvalidUser();

        DeleteUserCommandHandler handler = new(_mockUserManager.Object);

        // Act
        Result result = await handler.Handle(new DeleteUserCommand { AccountNumber = user.AccountNumber }, CancellationToken.None);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Contains("Invalid User", result.Message);
    }

    [Fact]

    public async Task Handle_EmptyRequest_ReturnsFailure()
    {
        // Arrange
        DeleteUserCommandHandler handler = new(_mockUserManager.Object);

        // Act
        Result result = await handler.Handle(new DeleteUserCommand { }, CancellationToken.None);

        // Assert
        _mockUserManager.Verify(x => x.DeleteAsync(It.IsAny<User>()), Times.Never);
        Assert.False(result.Succeeded);
        Assert.Contains("Invalid User", result.Message);
    }

}
