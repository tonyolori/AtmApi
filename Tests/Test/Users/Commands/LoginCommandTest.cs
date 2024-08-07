using Application.Common.Models;
using Application.Interfaces;
using Application.Users.Commands;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Security.Claims;
using Test.Data;
using Test.Mocks;

namespace Test.Users.Commands
{
    public class LoginCommandTest
    {
        private readonly Mock<IAuthService> _mockAuthHelper;
        private readonly Mock<UserManager<User>> _mockUserManager;
        private readonly Mock<SignInManager<User>> _mockSignInManager;

        private readonly List<User> _users;

        public LoginCommandTest()
        {
            _mockAuthHelper = new Mock<IAuthService>();

            _users = UserFaker.GenerateValidUsers();
            DbSet<User> DbUsers = DbContextMock.GetQueryableMockDbSet(_users);

            _mockUserManager = UserManagerMock.MockUserManager<User>();
            _mockUserManager.Setup(x => x.Users).Returns(DbUsers);

            _mockSignInManager = new Mock<SignInManager<User>>(
            _mockUserManager.Object,
            Mock.Of<IHttpContextAccessor>(),
            Mock.Of<IUserClaimsPrincipalFactory<User>>(), null, null, null, null);
        }

        [Fact]
        public async Task Handle_ValidEmailAndPassword_ReturnsSuccess()
        {
            //Arrange 
            User validUser = UserFaker.GenerateValidUser();

            _mockAuthHelper.Setup(x => x.GenerateJWTToken(It.IsAny<List<Claim>>())).Returns("This is a Valid Jwt Token");
            _mockUserManager.Setup(x => x.FindByEmailAsync(It.Is<string>(email => email == validUser.Email)))
                    .ReturnsAsync(validUser);
            _mockUserManager.Setup(x => x.GetRolesAsync(It.IsAny<User>()))
                    .Returns(Task.FromResult<IList<string>>(["Admin"]));
            _mockSignInManager.Setup(x => x.PasswordSignInAsync(It.IsAny<User>(), It.IsAny<string>(),
                It.IsAny<bool>(), It.IsAny<bool>())).Returns(Task.FromResult(SignInResult.Success));


            LoginCommandHandler handler = new(_mockAuthHelper.Object, _mockUserManager.Object, _mockSignInManager.Object);

            //Act 
            Result result = await handler.Handle(new LoginCommand { Email = validUser.Email, Password = validUser.PasswordHash }, CancellationToken.None);

            //Assert
            Assert.True(result.Succeeded);
            Assert.Contains("This is a Valid Jwt Token", result.Message);

        }


        [Fact]
        public async Task Handle_EmailNotFound_ReturnsFailure()
        {
            //Arrange 
            User validUser = UserFaker.GenerateValidUser();

            _mockAuthHelper.Setup(x => x.GenerateJWTToken(It.IsAny<List<Claim>>())).Returns("This is a Valid Jwt Token");
            _mockUserManager.Setup(x => x.FindByEmailAsync(It.Is<string>(email => email == validUser.Email)))
                    .ReturnsAsync(validUser);
            _mockUserManager.Setup(x => x.GetRolesAsync(It.IsAny<User>()))
                    .Returns(Task.FromResult<IList<string>>(["Admin"]));
            _mockSignInManager.Setup(x => x.PasswordSignInAsync(It.IsAny<User>(), It.IsAny<string>(),
                It.IsAny<bool>(), It.IsAny<bool>())).Returns(Task.FromResult(SignInResult.Success));


            LoginCommandHandler handler = new(_mockAuthHelper.Object, _mockUserManager.Object, _mockSignInManager.Object);

            //Act 
            Result result = await handler.Handle(new LoginCommand { Email = "ThisIsNotAnEmailInTheDatabase@Test.com", Password = validUser.PasswordHash }, CancellationToken.None);

            //Assert
            Assert.False(result.Succeeded);
            Assert.Contains("User not found", result.Message);

        }

        [Fact]
        public async Task Handle_ValidationFailure_ReturnsFailure()
        {
            //Arrange 
            User invalidUser = UserFaker.GenerateInvalidUser();

            _mockAuthHelper.Setup(x => x.GenerateJWTToken(It.IsAny<List<Claim>>())).Returns("This is a Valid Jwt Token");
            _mockUserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                    .ReturnsAsync((User?)null);
            _mockUserManager.Setup(x => x.GetRolesAsync(It.IsAny<User>()))
                    .Returns(Task.FromResult<IList<string>>(["Admin"]));
            _mockSignInManager.Setup(x => x.PasswordSignInAsync(It.IsAny<User>(), It.IsAny<string>(),
                It.IsAny<bool>(), It.IsAny<bool>())).Returns(Task.FromResult(SignInResult.Success));


            LoginCommandHandler handler = new(_mockAuthHelper.Object, _mockUserManager.Object, _mockSignInManager.Object);

            //Act 
            Result result = await handler.Handle(new LoginCommand { Email = invalidUser.Email, Password = invalidUser.PasswordHash }, CancellationToken.None);

            //Assert
            Assert.False(result.Succeeded);
            Assert.Contains("Invalid email format", result.Message);
            Assert.Contains("Password must be at least 8 characters long", result.Message);
            Assert.Contains("Password must contain at least one number", result.Message);

        }
        [Fact]
        public async Task Handle_SignIn_AccountLockedOut()
        {
            // Arrange
            User user = UserFaker.GenerateValidUser();
            _mockUserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(user);
            _mockSignInManager.Setup(x => x.PasswordSignInAsync(It.IsAny<User>(), It.IsAny<string>(),
                It.IsAny<bool>(), It.IsAny<bool>())).Returns(Task.FromResult(SignInResult.LockedOut));

            LoginCommandHandler handler = new(_mockAuthHelper.Object, _mockUserManager.Object, _mockSignInManager.Object);

            // Act
            Result result = await handler.Handle(new LoginCommand { Email = user.Email, Password = user.PasswordHash }, CancellationToken.None);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Your account is locked out. Please try again later.", result.Message);
        }

        [Fact]
        public async Task Handle_SignIn_AccountNotAllowed()
        {
            // Arrange
            User user = UserFaker.GenerateValidUser();
            _mockUserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(user);
            _mockSignInManager.Setup(x => x.PasswordSignInAsync(It.IsAny<User>(), It.IsAny<string>(),
                It.IsAny<bool>(), It.IsAny<bool>())).Returns(Task.FromResult(SignInResult.LockedOut));

            LoginCommandHandler handler = new(_mockAuthHelper.Object, _mockUserManager.Object, _mockSignInManager.Object);

            // Act
            Result result = await handler.Handle(new LoginCommand { Email = user.Email, Password = user.PasswordHash }, CancellationToken.None);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Contains("Your account is locked out. Please try again later", result.Message);
        }

        [Fact]
        public async Task Handle_InvalidCredentials_ReturnsFailure()
        {
            // Arrange
            User user = UserFaker.GenerateValidUser();
            _mockUserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(user);
            _mockSignInManager.Setup(x => x.PasswordSignInAsync(It.IsAny<User>(), It.IsAny<string>(),
                It.IsAny<bool>(), It.IsAny<bool>())).Returns(Task.FromResult(SignInResult.Failed));

            LoginCommandHandler handler = new(_mockAuthHelper.Object, _mockUserManager.Object, _mockSignInManager.Object);

            // Act
            Result result = await handler.Handle(new() { Email = user.Email, Password = "wrong Password1*" }, CancellationToken.None);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Contains("Invalid Email or password", result.Message);
        }

    }


}
