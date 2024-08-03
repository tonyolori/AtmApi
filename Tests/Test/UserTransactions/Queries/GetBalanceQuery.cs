using Application.Common.Models;
using Application.UserTransactions.Queries;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Moq;
using Test.Data;
using Test.Mocks;

namespace Application.UnitTest.UserTransactions.Queries;
public class GetBalanceQueryHandlerTest
{
    private readonly Mock<UserManager<User>> _mockUserManager;
    private readonly List<User> _users;

    public GetBalanceQueryHandlerTest()
    {
        _users = UserFaker.GenerateValidUsers();

        _mockUserManager = UserManagerMock.MockUserManager<User>();
        _mockUserManager.Setup(x => x.Users).Returns(DbContextMock.GetQueryableMockDbSet(_users));
    }

    [Fact]
    public async Task Handle_ValidAccountNumber_ReturnsBalance()
    {
        // Arrange
        User user = UserFaker.GenerateValidUser();
        long accountNumber = user.AccountNumber;


        GetBalanceHandler handler = new(_mockUserManager.Object);

        // Act
        Result result = await handler.Handle(new GetBalanceQuery { AccountNumber = accountNumber }, CancellationToken.None);

        // Assert
        Assert.True(result.Succeeded);
        Assert.Equal(user.Balance, (long)result.Entity);
    }

    [Fact]
    public async Task Handle_InvalidAccountNumber_ReturnsFailure()
    {
        // Arrange
        long accountNumber = UserFaker.GenerateInvalidUser().AccountNumber;


        GetBalanceHandler handler = new(_mockUserManager.Object);

        // Act
        Result result = await handler.Handle(new GetBalanceQuery { AccountNumber = accountNumber }, CancellationToken.None);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Contains("User not found", result.Message);
    }
}

