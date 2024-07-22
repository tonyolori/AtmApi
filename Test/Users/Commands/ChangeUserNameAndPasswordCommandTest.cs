using Application.Interfaces;
using Application.Users.Commands;
using Domain.Models;
using Moq;
using Test.Data;
using Test.Mocks;

namespace Test.Users.Commands;

public class ChangeUserNameAndPasswordCommandTest
{
    private readonly Mock<IDataContext> _mockContext;
    private readonly Mock<ISecretHasher> _mockSecretHasher;
    private readonly List<User> _users;

    public ChangeUserNameAndPasswordCommandTest()
    {
        _mockContext = new Mock<IDataContext>();
        _mockSecretHasher = new Mock<ISecretHasher>();
        _users = UserFaker.GenerateUsers();

        var DbUsers = DbContextMock.GetQueryableMockDbSet(_users);
        _mockContext.Setup(x => x.Users).Returns(DbUsers);
        _mockSecretHasher.Setup(x => x.Hash(It.IsAny<string>())).Returns("12345678*a");
    }

    [Fact]
    public async Task Handle_ValidUsers_ReturnsSuccess()
    {
        // Arrange
        var handler = new ChangeUserNameAndPasswordCommandHandler(_mockContext.Object, _mockSecretHasher.Object);

        foreach (User user in _users)
        {
            var userDto = UserFaker.GenerateValidDto();
            userDto.Email = user.Email; //ensure Existing Email is being used

            // Act
            var result = await handler.Handle(new ChangeUserNameAndPasswordCommand(userDto), CancellationToken.None);

            // Assert
            Assert.True(result.Succeeded);
        }
    }

    [Fact]
    public async Task Handle_InvalidUser_ReturnsFailure()
    {
        // Arrange
        var userDto = UserFaker.GenerateInvalidDto();
        userDto.Email = _users[0].Email; // ensure existing user

        var handler = new ChangeUserNameAndPasswordCommandHandler(_mockContext.Object, _mockSecretHasher.Object);

        // Act
        var result = await handler.Handle(new ChangeUserNameAndPasswordCommand(userDto), CancellationToken.None);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Contains("Invalid Password", result.Message);
        Assert.Contains("First and last name cannot be the same", result.Message);
    }


    [Fact]
    public async Task Handle_ExistingUserSamePassword_ReturnsFailure()
    {
        // Arrange
        // ensure password Hash matches existing password
        _mockSecretHasher.Setup(x => x.Hash(It.IsAny<string>())).Returns(_users[0].Password); 
        
        var userDto = UserFaker.GenerateValidDto();
        userDto.Email = _users[0].Email; // ensure existing user

        var handler = new ChangeUserNameAndPasswordCommandHandler(_mockContext.Object, _mockSecretHasher.Object);

        // Act
        var result = await handler.Handle(new ChangeUserNameAndPasswordCommand(userDto), CancellationToken.None);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Contains("New password cannot be the same as old password", result.Message);
    }


    [Fact]
    public async Task Handle_NonexistentUser_ReturnsFailure()
    {
        // Arrange
        _mockSecretHasher.Setup(x => x.Hash(It.IsAny<string>())).Returns("existingPassword"); // Hash matches existing password

        var userDto = UserFaker.GenerateValidDto();
        userDto.Email = "createInvalidUserForTestExample@test.com"; // ensure non-existent user

        var handler = new ChangeUserNameAndPasswordCommandHandler(_mockContext.Object, _mockSecretHasher.Object);

        // Act
        var result = await handler.Handle(new ChangeUserNameAndPasswordCommand(userDto), CancellationToken.None);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Contains("user not found", result.Message);
    }
}
