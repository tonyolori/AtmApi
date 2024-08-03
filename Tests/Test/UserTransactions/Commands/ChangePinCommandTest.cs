using Application.Common.Models;
using Application.Interfaces;
using Application.UserTransactions.Commands;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Moq;
using Test.Data;
using Test.Mocks;

namespace Application.UnitTest.UserTransactions.Commands
{
    public class ChangePinCommandHandlerTest
    {
        private readonly Mock<ISecretHasherService> _mockSecretHasher;
        private readonly List<User> _users;
        private readonly Mock<UserManager<User>> _mockUserManager;

        public ChangePinCommandHandlerTest()
        {
            _mockSecretHasher = new Mock<ISecretHasherService>();

            _users = UserFaker.GenerateValidUsers();

            _mockUserManager = UserManagerMock.MockUserManager<User>();
            _mockUserManager.Setup(x => x.Users).Returns(DbContextMock.GetQueryableMockDbSet(_users));

        }

        [Fact]
        public async Task Handle_ValidPinChange_ReturnsSuccess()
        {
            // Arrange
            User user = UserFaker.GenerateValidUser();
            int newPin = 4321;
            string hashedNewPin = "NewPinHash";

            _mockSecretHasher.Setup(x => x.Hash(It.Is<string>(p => p == newPin.ToString())))
            .Returns(hashedNewPin);

            _mockUserManager.Setup(x => x.UpdateAsync(It.IsAny<User>())).Returns(Task.FromResult(IdentityResult.Success));

            ChangePinCommandHandler handler = new(_mockSecretHasher.Object, _mockUserManager.Object);

            // Act
            Result result = await handler.Handle(new ChangePinCommand
            {
                AccountNumber = user.AccountNumber,
                OldPin = user.Pin,
                NewPin = 4321
            }, CancellationToken.None);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Contains("Updated successfully", result.Message);
        }

        [Fact]
        public async Task Handle_InvalidOldPin_ReturnsFailure()
        {
            // Arrange
            User user = UserFaker.GenerateValidUser();
            int newPin = 4321;
            string hashedNewPin = "NewPinHash";

            _mockSecretHasher.Setup(x => x.Hash(It.Is<string>(p => p == newPin.ToString())))
            .Returns(hashedNewPin);

            _mockUserManager.Setup(x => x.UpdateAsync(It.IsAny<User>())).Returns(Task.FromResult(IdentityResult.Success));

            ChangePinCommandHandler handler = new(_mockSecretHasher.Object, _mockUserManager.Object);

            // Act
            Result result = await handler.Handle(new ChangePinCommand
            {
                AccountNumber = user.AccountNumber,
                OldPin = "wrong pin",//wrong Pin
                NewPin = 4321
            }, CancellationToken.None);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Invalid Pin", result.Message);
        }

        [Fact]
        public async Task Handle_NewPinSameAsOldPin_ReturnsFailure()
        {
            // Arrange
            User user = UserFaker.GenerateValidUser();
            int newPin = 4321;
            string hashedNewPin = "NewPinHash";

            _mockSecretHasher.Setup(x => x.Hash(It.Is<string>(p => p == newPin.ToString())))
            .Returns(user.Pin); //returns the same as the old pin

            ChangePinCommandHandler handler = new(_mockSecretHasher.Object, _mockUserManager.Object);

            // Act
            Result result = await handler.Handle(new ChangePinCommand
            {
                AccountNumber = user.AccountNumber,
                OldPin = user.Pin,
                NewPin = 4321
            }, CancellationToken.None);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("New password cannot be the same as old password", result.Message);
        }

        [Fact]
        public async Task Handle_UserNotFound_ReturnsFailure()
        {
            // Arrange
            User user = UserFaker.GenerateInvalidUser();

            ChangePinCommandHandler handler = new(_mockSecretHasher.Object, _mockUserManager.Object);

            // Act
            Result result = await handler.Handle(new ChangePinCommand
            {
                AccountNumber = user.AccountNumber,
                OldPin = user.Pin,
                NewPin = 4321
            }, CancellationToken.None);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("user not found", result.Message);
        }
    }
}