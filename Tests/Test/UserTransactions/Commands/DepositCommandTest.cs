using Application.Common.Models;
using Application.UserTransactions.Commands;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Moq;
using Test.Data;
using Test.Mocks;

namespace Application.UnitTest.UserTransactions.Commands
{
    public class DepositCommandTest
    {
        private readonly Mock<UserManager<User>> _mockUserManager;
        private readonly List<User> _users;

        public DepositCommandTest()
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
            uint depositAmount = 100;

            _mockUserManager.Setup(x => x.UpdateAsync(It.IsAny<User>()))
            .Returns(Task.FromResult(IdentityResult.Success));

            DepositCommandHandler handler = new(_mockUserManager.Object);

            // Act
            Result result = await handler.Handle(new DepositCommand { AccountNumber = accountNumber, Amount = depositAmount }, CancellationToken.None);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Contains("Deposit successful", result.Message);
        }

        [Fact]
        public async Task Handle_InvalidAccountNumber_ReturnsFailure()
        {
            // Arrange
            User user = UserFaker.GenerateInvalidUser();
            long accountNumber = user.AccountNumber;
            uint depositAmount = 100;

            DepositCommandHandler handler = new(_mockUserManager.Object);

            // Act
            Result result = await handler.Handle(new DepositCommand { AccountNumber = accountNumber, Amount = depositAmount }, CancellationToken.None);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Contains("user not found", result.Message);
        }
    }

}
