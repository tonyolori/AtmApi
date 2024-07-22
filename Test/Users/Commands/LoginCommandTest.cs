using Application.Common.DTOs;
using Application.Interfaces;
using Application.Users.Commands;
using Domain.Models;
using Moq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test.Data;
using Test.Mocks;

namespace Test.Users.Commands
{
    public class LoginCommandTest
    {
        private readonly Mock<IDataContext> _mockContext;
        private readonly Mock<IAuthHelper> _mockAuthHelper;
        private readonly Mock<ISecretHasher> _mockSecretHasher;

        private readonly List<User> _users;

        public LoginCommandTest()
        {
            _mockContext = new Mock<IDataContext>();
            _mockAuthHelper = new Mock<IAuthHelper>();
            _mockSecretHasher = new Mock<ISecretHasher>();

            _users = UserFaker.GenerateUsers();

            var DbUsers = DbContextMock.GetQueryableMockDbSet(_users);

            _mockContext.Setup(x=>x.Users).Returns(DbUsers);
            _mockSecretHasher.Setup(x => x.Hash(It.IsAny<string>())).Returns("12345678*a");

        }

        [Fact]
        public async Task Handle_ValidEmailAndPassword_ReturnsSuccess()
        {
            //Arrange 
            _mockAuthHelper.Setup(x => x.GenerateJWTToken(It.IsAny<User>())).Returns("This is a Valid Jwt Token");
            _mockSecretHasher.Setup(x => x.Verify(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            var handler = new LoginCommandHandler(_mockContext.Object, _mockAuthHelper.Object, _mockSecretHasher.Object);

            foreach(User user in _users)
            {
                LoginDto login = new() { Email= user.Email , Password = user.Password };

                //Act 
                var result = await handler.Handle(new LoginCommand(login), CancellationToken.None);

                //Assert
                Assert.True(result.Succeeded);
                Assert.Contains("This is a Valid Jwt Token", result.Message);
            }

        }


        [Fact]
        public async Task Handle_InvalidEmail_ReturnsFailure()
        {
            //Arrange 
            _mockAuthHelper.Setup(x => x.GenerateJWTToken(It.IsAny<User>())).Returns("This is a Valid Jwt Token");
            _mockSecretHasher.Setup(x => x.Verify(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            var handler = new LoginCommandHandler(_mockContext.Object, _mockAuthHelper.Object, _mockSecretHasher.Object);

            LoginDto login = new() { Email = "ThisIsNotAnEmailInTheDatabase@Test.com", Password = _users[0].Password };

            //Act 
            var result = await handler.Handle(new LoginCommand(login), CancellationToken.None);

            //Assert
            Assert.False(result.Succeeded);
            Assert.Contains("User does not exist", result.Message);

        }

    }
}
