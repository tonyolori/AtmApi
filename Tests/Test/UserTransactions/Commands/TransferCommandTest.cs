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
    public class TransferHandlerTest
    {
        private readonly Mock<UserManager<User>> _mockUserManager;
        private readonly Mock<ISecretHasherService> _mockSecretHasher;
        private readonly List<User> _users;

        public TransferHandlerTest()
        {
            _mockSecretHasher = new Mock<ISecretHasherService>();
            _users = UserFaker.GenerateValidUsers();
            _mockUserManager = UserManagerMock.MockUserManager<User>();
            _mockUserManager.Setup(x => x.Users).Returns(DbContextMock.GetQueryableMockDbSet(_users));
        }

        [Fact]
        public async Task Handle_ValidTransfer_ReturnsSuccess()
        {
            // Arrange
            User sender = UserFaker.GenerateValidUser();
            User receiver = _users[1];
            uint transferAmount = 100;
            string hashedPin = sender.Pin;

            _mockSecretHasher.Setup(x => x.Hash(It.IsAny<string>()))
                .Returns(hashedPin);
            _mockUserManager.Setup(x => x.UpdateAsync(It.IsAny<User>()))
                .Returns(Task.FromResult(IdentityResult.Success));

            TransferHandler handler = new(_mockUserManager.Object, _mockSecretHasher.Object);

            // Act
            Result result = await handler.Handle(new TransferCommand
            {
                SendingAccount = sender.AccountNumber,
                ReceivingAccount = receiver.AccountNumber,
                Amount = transferAmount,
                Pin = 1234
            }, CancellationToken.None);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Contains("Request was executed successfully", result.Message);
        }


        [Fact]
        public async Task Handle_InvalidPin_ReturnsFailure()
        {
            // Arrange
            User sender = UserFaker.GenerateValidUser();
            User receiver = _users[1];
            uint transferAmount = 100;
            string hashedPin = sender.Pin;

            _mockSecretHasher.Setup(x => x.Hash(It.Is<string>(p => p == sender.Pin)))
                .Returns("invalidPin");

            TransferHandler handler = new(_mockUserManager.Object, _mockSecretHasher.Object);

            // Act
            Result result = await handler.Handle(new TransferCommand
            {
                SendingAccount = sender.AccountNumber,
                ReceivingAccount = receiver.AccountNumber,
                Amount = transferAmount,
                Pin = 1234
            }, CancellationToken.None);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Contains("Invalid Pin", result.Message);
        }

        [Fact]
        public async Task Handle_TransferToSameAccount_ReturnsFailure()
        {
            // Arrange
            User sender = UserFaker.GenerateValidUser();
            User receiver = sender;
            uint transferAmount = 100;
            string hashedPin = sender.Pin;

            _mockSecretHasher.Setup(x => x.Hash(It.Is<string>(p => p == "1234")))
                .Returns(hashedPin);

            TransferHandler handler = new(_mockUserManager.Object, _mockSecretHasher.Object);

            // Act
            Result result = await handler.Handle(new TransferCommand
            {
                SendingAccount = sender.AccountNumber,
                ReceivingAccount = receiver.AccountNumber,
                Amount = transferAmount,
                Pin = 1234
            }, CancellationToken.None);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Contains("Cannot Transfer to the same account", result.Message);
        }

        [Fact]
        public async Task Handle_InvalidReceivingUser_ReturnsFailure()
        {
            // Arrange
            User sender = UserFaker.GenerateValidUser();
            User receiver = UserFaker.GenerateInvalidUser();
            uint transferAmount = 100;
            string hashedPin = sender.Pin;

            _mockSecretHasher.Setup(x => x.Hash(It.Is<string>(p => p == "1234")))
                .Returns(hashedPin);

            TransferHandler handler = new(_mockUserManager.Object, _mockSecretHasher.Object);

            // Act
            Result result = await handler.Handle(new TransferCommand
            {
                SendingAccount = sender.AccountNumber,
                ReceivingAccount = receiver.AccountNumber,
                Amount = transferAmount,
                Pin = 1234
            }, CancellationToken.None);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Contains("Invalid Receiving user", result.Message);
        }

        [Fact]
        public async Task Handle_InsufficientFunds_ReturnsFailure()
        {
            // Arrange
            User sender = UserFaker.GenerateValidUser();
            User receiver = _users[1];
            uint transferAmount = 999999999; //large amount
            string hashedPin = sender.Pin;

            _mockSecretHasher.Setup(x => x.Hash(It.IsAny<string>()))
                .Returns(hashedPin);

            TransferHandler handler = new(_mockUserManager.Object, _mockSecretHasher.Object);

            // Act
            Result result = await handler.Handle(new TransferCommand
            {
                SendingAccount = sender.AccountNumber,
                ReceivingAccount = receiver.AccountNumber,
                Amount = transferAmount,
                Pin = 1234
            }, CancellationToken.None);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Contains("insufficient funds", result.Message);
        }
    }
}
