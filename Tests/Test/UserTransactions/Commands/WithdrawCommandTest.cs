using Application.Common.Models;
using Application.UserTransactions.Commands;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Moq;
using Test.Data;
using Test.Mocks;

namespace Application.UnitTest.UserTransactions.Commands
{
    public class WithdrawCommandTest
    {
        private readonly Mock<UserManager<User>> _mockUserManager;
        private readonly List<User> _users;

        public WithdrawCommandTest()
        {
            _users = UserFaker.GenerateValidUsers();

            _mockUserManager = UserManagerMock.MockUserManager<User>();
            _mockUserManager.Setup(x => x.Users).Returns(DbContextMock.GetQueryableMockDbSet(_users));
        }

        [Fact]
        public async Task Handle_ValidAccountNumber_ReturnsSuccess()
        {
            // Arrange
            User user = UserFaker.GenerateValidUser();
            long accountNumber = user.AccountNumber;
            uint WithdrawAmount = 10;

            _mockUserManager.Setup(x => x.UpdateAsync(It.IsAny<User>()))
            .Returns(Task.FromResult(IdentityResult.Success));

            WithdrawCommandHandler handler = new(_mockUserManager.Object);

            // Act
            Result result = await handler.Handle(new WithdrawCommand { AccountNumber = accountNumber, Amount = WithdrawAmount }, CancellationToken.None);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Contains("withdraw Successful", result.Message);
        }

        [Fact]
        public async Task Handle_InvalidAccountNumber_ReturnsFailure()
        {
            // Arrange
            User user = UserFaker.GenerateInvalidUser();
            long accountNumber = user.AccountNumber;
            uint WithdrawAmount = 100;

            WithdrawCommandHandler handler = new(_mockUserManager.Object);

            // Act
            Result result = await handler.Handle(new WithdrawCommand { AccountNumber = accountNumber, Amount = WithdrawAmount }, CancellationToken.None);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Contains("User not found", result.Message);
        }

        [Fact]
        public async Task Handle_InvalidInsufficientFunds_ReturnsFailure()
        {
            // Arrange
            User user = UserFaker.GenerateValidUser();
            long accountNumber = user.AccountNumber;
            uint WithdrawAmount = 99999999;//large amount insufficient funds

            WithdrawCommandHandler handler = new(_mockUserManager.Object);

            // Act
            Result result = await handler.Handle(new WithdrawCommand { AccountNumber = accountNumber, Amount = WithdrawAmount }, CancellationToken.None);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Contains("Insufficient Funds", result.Message);
        }
    }

}
