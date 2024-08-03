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
    public class ChangeNameAndPasswordCommandHandlerTest
    {
        private readonly Mock<ISecretHasherService> _mockSecretHasher;
        private readonly List<User> _users;
        private readonly Mock<UserManager<User>> _mockUserManager;

        public ChangeNameAndPasswordCommandHandlerTest()
        {
            _mockSecretHasher = new Mock<ISecretHasherService>();

            _users = UserFaker.GenerateValidUsers();
            _mockUserManager = UserManagerMock.MockUserManager<User>();
            _mockUserManager.Setup(x => x.Users).Returns(DbContextMock.GetQueryableMockDbSet(_users));
        }

        [Fact]
        public async Task Handle_ValidNameAndPasswordChange_ReturnsSuccess()
        {
            // Arrange
            User user = UserFaker.GenerateValidUser();
            string newFirstName = "NewFirstName";
            string newLastName = "NewLastName";
            string newPassword = "123456789*";
            string hashedNewPassword = "HashedNewPassword";
            int pin = 1234;

            _mockSecretHasher.Setup(x => x.Hash(It.Is<string>(p => p == pin.ToString())))
                .Returns(hashedNewPassword);
            _mockUserManager.Setup(x => x.FindByEmailAsync(user.Email))
           .ReturnsAsync(user);
            _mockUserManager.Setup(x => x.UpdateAsync(It.IsAny<User>()))
                .Returns(Task.FromResult(IdentityResult.Success));

            ChangeNameAndPasswordCommandHandler handler = new(_mockUserManager.Object, _mockSecretHasher.Object);

            // Act
            Result result = await handler.Handle(new ChangeNameAndPasswordCommand
            {
                User = new UserDto
                {
                    Email = user.Email,
                    FirstName = newFirstName,
                    LastName = newLastName,
                    Password = newPassword,
                    Pin = pin
                }
            }, CancellationToken.None);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Contains("Updated successfully", result.Message);
        }

        [Fact]
        public async Task Handle_InvalidEmail_ReturnsFailure()
        {
            // Arrange
            User user = UserFaker.GenerateValidUser();
            string newFirstName = "NewFirstName";
            string newLastName = "NewLastName";
            string newPassword = "123456789*";

            _mockUserManager.Setup(x => x.FindByEmailAsync(user.Email))
           .ReturnsAsync((User)null); //email not found

            ChangeNameAndPasswordCommandHandler handler = new(_mockUserManager.Object, _mockSecretHasher.Object);

            // Act
            Result result = await handler.Handle(new ChangeNameAndPasswordCommand
            {
                User = new UserDto
                {
                    Email = user.Email,
                    FirstName = newFirstName,
                    LastName = newLastName,
                    Password = newPassword,
                    Pin = 1234
                }
            }, CancellationToken.None);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Contains("user not found", result.Message);
        }

        [Fact]
        public async Task Handle_SamePassword_ReturnsFailure()
        {
            // Arrange
            User user = UserFaker.GenerateValidUser();
            string newFirstName = "NewFirstName";
            string newLastName = "NewLastName";
            string newPassword = "123456789*";
            string hashedNewPassword = "HashedNewPassword";

            _mockSecretHasher.Setup(x => x.Hash(It.Is<string>(p => p == newPassword)))
                .Returns(user.PasswordHash);
            _mockUserManager.Setup(x => x.FindByEmailAsync(user.Email))
           .ReturnsAsync(user);

            ChangeNameAndPasswordCommandHandler handler = new(_mockUserManager.Object, _mockSecretHasher.Object);

            // Act
            Result result = await handler.Handle(new ChangeNameAndPasswordCommand
            {
                User = new UserDto
                {
                    Email = user.Email,
                    FirstName = newFirstName,
                    LastName = newLastName,
                    Password = newPassword,
                    Pin = 1234
                }
            }, CancellationToken.None);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Contains("New password cannot be the same as old password", result.Message);
        }

        [Fact]
        public async Task Handle_InvalidNewPassword_ReturnsFailure()
        {
            // Arrange
            User user = UserFaker.GenerateValidUser();
            string newFirstName = "NewFirstName";
            string newLastName = "NewLastName";
            string newPassword = "newPassword"; //will not get through password validator
            string hashedNewPassword = "HashedNewPassword";


            ChangeNameAndPasswordCommandHandler handler = new(_mockUserManager.Object, _mockSecretHasher.Object);

            // Act
            Result result = await handler.Handle(new ChangeNameAndPasswordCommand
            {
                User = new UserDto
                {
                    Email = user.Email,
                    FirstName = newFirstName,
                    LastName = newLastName,
                    Password = newPassword,
                    Pin = 1234
                }
            }, CancellationToken.None);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Contains("Password must contain at least one number", result.Message);
        }
    }
}

