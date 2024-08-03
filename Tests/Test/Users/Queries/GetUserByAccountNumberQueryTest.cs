using Application.Common.Models;
using Application.Users.Queries;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using Test.Data;
using Test.Mocks;

namespace Test.Users.Queries;
public class GetUserByAccountNumberQueryHandlerTest
{
    private readonly Mock<UserManager<User>> _mockUserManager;

    public GetUserByAccountNumberQueryHandlerTest()
    {
        List<User> users = UserFaker.GenerateValidUsers();

        _mockUserManager = UserManagerMock.MockUserManager<User>();

        DbSet<User> DbUsers = DbContextMock.GetQueryableMockDbSet(users);
        _mockUserManager.Setup(x => x.Users).Returns(DbUsers);
    }

    [Fact]
    public async Task HandleValidUser_ReturnsUser_WhenUserFound()
    {
        // Arrange
        User expectedUser = UserFaker.GenerateValidUser();

        GetUserByAccountNumberQueryHandler handler = new(_mockUserManager.Object);

        // Act
        Result result = await handler.Handle(new() { AccountNumber = expectedUser.AccountNumber }, CancellationToken.None);
        User userResult = (User)result.Entity;

        // Assert
        Assert.True(result.Succeeded);
        Assert.Equal(userResult.AccountNumber, expectedUser.AccountNumber);
    }

    [Fact]
    public async Task Handle_ReturnsNotFound_WhenUserNotFound()
    {
        // Arrange
        GetUserByAccountNumberQueryHandler handler = new(_mockUserManager.Object);

        // Act
        Result result = await handler.Handle(new() { AccountNumber = 12345 }, CancellationToken.None);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Contains("User not found", result.Message);
        Assert.Null(result.Entity);
    }
}