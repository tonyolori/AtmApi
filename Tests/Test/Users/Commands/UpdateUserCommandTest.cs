using Application.Common.Models;
using Application.Users.Commands;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using Test.Data;
using Test.Mocks;

namespace Test.Users.Commands;

public class UpdateUserCommandTest
{
    private readonly List<User> _users;
    private readonly Mock<UserManager<User>> _mockUserManager;


    public UpdateUserCommandTest()
    {
        _users = UserFaker.GenerateValidUsers();


        DbSet<User> DbUsers = DbContextMock.GetQueryableMockDbSet(_users);
        _mockUserManager = UserManagerMock.MockUserManager<User>();
        _mockUserManager.Setup(x => x.Users).Returns(DbUsers);

    }

    [Fact]
    public async Task Handle_UpdatesUser_ReturnsSuccess()
    {
        // Arrange
        User user = UserFaker.GenerateValidUser();
        UpdateUserCommand updateCommand = new() { User = user };

        _mockUserManager.Setup(x => x.UpdateAsync(It.IsAny<User>()))
            .Returns(Task.FromResult(IdentityResult.Success));

        UpdateUserCommandHandler handler = new(_mockUserManager.Object);

        // Act
        Result result = await handler.Handle(updateCommand, CancellationToken.None);

        // Assert
        _mockUserManager.Verify(x => x.UpdateAsync(It.Is<User>(u => u.Id == user.Id && u.Email == user.Email)), Times.Once);
        Assert.True(result.Succeeded);
    }

    [Fact]
    public async Task Handle_InvalidUser_ReturnsFailure()
    {
        // Arrange
        User user = UserFaker.GenerateValidUser();
        UpdateUserCommand updateCommand = new() { User = user };

        _mockUserManager.Setup(x => x.UpdateAsync(It.IsAny<User>()))
            .Returns(Task.FromResult(IdentityResult.Failed()));

        UpdateUserCommandHandler handler = new(_mockUserManager.Object);

        // Act
        Result result = await handler.Handle(updateCommand, CancellationToken.None);

        // Assert
        Assert.False(result.Succeeded);
    }
}
