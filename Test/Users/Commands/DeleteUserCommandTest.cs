using Application.Interfaces;
using Application.Users.Commands;
using Domain.Models;
using Moq;
using Test.Data;
using Test.Mocks;

namespace Test.Users.Commands;

public class DeleteUserCommandTest
{
    private readonly Mock<IDataContext> _mockContext;
    private readonly List<User> _users;

    public DeleteUserCommandTest()
    {
        _mockContext = new Mock<IDataContext>();
        _users = UserFaker.GenerateUsers();

        var DbUsers = DbContextMock.GetQueryableMockDbSet(_users);
        _mockContext.Setup(x => x.Users).Returns(DbUsers);
    }

    [Fact]
    public async Task Handle_ValidUsers_ReturnsSuccess()
    {
        // Arrange
        var handler = new DeleteUserCommandHandler(_mockContext.Object);

        foreach (User user in _users)
        {
            // Act
            var result = await handler.Handle(new DeleteUserCommand(user.AccountNumber), CancellationToken.None);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Contains("User Deleted", result.Message);
        }
    }

    [Fact]
    public async Task Handle_NonexistentUser_ReturnsFailure()
    {
        // Arrange
        var handler = new DeleteUserCommandHandler(_mockContext.Object);

        // Act
        //9 digit 0 account number for test
        var result = await handler.Handle(new DeleteUserCommand(00000000000), CancellationToken.None);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Contains("Invalid User", result.Message);
    }
}
