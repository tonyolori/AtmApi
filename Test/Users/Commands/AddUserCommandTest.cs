using Application.Extensions;
using Application.Interfaces;
using Application.Users.Commands;
using Domain.Models;
using Moq;
using Test.Data;
using Test.Mocks;

namespace Test.Users.Commands;
public class AddUserCommandHandlerTests
{
    private readonly Mock<IDataContext> _mockContext;
    private readonly Mock<ISecretHasher> _mockSecretHasher;
    private readonly Mock<IEmailSender> _mockEmailSender;
    private readonly List<User> _users;
    public AddUserCommandHandlerTests()
    {
        _mockContext = new Mock<IDataContext>();
        _mockSecretHasher = new Mock<ISecretHasher>();
        _mockEmailSender = new Mock<IEmailSender>();
        _users = UserFaker.GenerateUsers();

        var DbUsers = DbContextMock.GetQueryableMockDbSet(_users);
        _mockContext.Setup(x => x.Users).Returns(DbUsers);
        _mockSecretHasher.Setup(x => x.Hash(It.IsAny<string>())).Returns("12345678*a");
    }


    [InlineData(10)] // Change the number of Users to test
    public async Task Handle_ValidUsers_ReturnSuccess(int iterations)
    {
        //Arrange
        var handler = new AddUserCommandHandler(_mockContext.Object, _mockSecretHasher.Object,_mockEmailSender.Object);

        for (int i = 0; i < iterations; i++)
        {
            // Act
            var expectedUser = UserFaker.GenerateValidDto();
            var result = await handler.Handle(new AddUserCommand(expectedUser, UserFaker.GetRole()), CancellationToken.None);

            // Assert
            Assert.True(result.Succeeded);
        }
    }

    [Fact]
    public async Task Handle_InvalidUser_ReturnsFailure()
    {
        // Arrange
        UserDto expectedUser = UserFaker.GenerateInvalidDto();
        var handler = new AddUserCommandHandler(_mockContext.Object, _mockSecretHasher.Object, _mockEmailSender.Object);

        // Act
        var result = await handler.Handle(new AddUserCommand(expectedUser, UserFaker.GetRole()), CancellationToken.None);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Contains("First and last name cannot be the same",result.Message);
        Assert.Contains("Invalid Password", result.Message);
    }

    [Fact]

    public async Task Handle_ExistingUser_ReturnsFailure()
    {
        // Arrange
        UserDto expectedUser = _users[0].ToUserDto();
        var handler = new AddUserCommandHandler(_mockContext.Object, _mockSecretHasher.Object, _mockEmailSender.Object);

        // Act
        var result = await handler.Handle(new AddUserCommand(expectedUser, UserFaker.GetRole()), CancellationToken.None);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Contains("User Already exists", result.Message);
    }
}


//public class AddUserCommandHandlerTest
//{
//    private readonly Mock<IDataContext> _fakeContext;
//    private readonly Mock<ISecretHasher> _fakeSecretHasher;
//    private readonly AddUserCommandHandler _handler;

//    public AddUserCommandHandlerTest()
//    {
//        _fakeContext = new Mock<IDataContext>();
//        _fakeSecretHasher = new Mock<ISecretHasher>();
//        _handler = new AddUserCommandHandler(_fakeContext.Object, _fakeSecretHasher.Object);
//    }

//    [Fact]
//    public async Task Handle_ValidUser_ReturnsSuccess()
//    {
//        // Arrange
//        var userDto = new UserDto { FirstName = "John", LastName = "Doe", Email = "test@email.com", Password = "P@ssw0rd", Pin = 123456 };
//        var userRole = UserRole.User;

//        _fakeSecretHasher.Setup(x => x.Hash(It.IsAny<string>())).Returns("HashedPassword");
//        _fakeContext.Setup(x => x.Users.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
//            .Returns((User user) => Task.FromResult(user));
//        _fakeContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult(1));

//        var doesUserExistQueryHandler = A.Fake<DoesUserExistQueryHandler>();
//        A.CallTo(() => doesUserExistQueryHandler.Handle(It.IsAny<DoesUserExistQuery>(), It.IsAny<CancellationToken>()))
//            .Returns(false);

//        var getUserByEmailQueryHandler = A.Fake<GetUserByEmailQueryHandler>();
//        A.CallTo(() => getUserByEmailQueryHandler.Handle(It.IsAny<GetUserByEmailQuery>(), It.IsAny<CancellationToken>()))
//            .Returns(Result.Success<User>(null));

//        _handler.GetType().GetProperty("_context", BindingFlags.NonPublic | BindingFlags.Instance)
//            .SetValue(_handler, _fakeContext.Object);
//        _handler.GetType().GetField("_secretHasher", BindingFlags.NonPublic | BindingFlags.Instance)
//            .SetValue(_handler, _fakeSecretHasher.Object);

//        // Act
//        var result = await _handler.Handle(new AddUserCommand(userDto, userRole), CancellationToken.None);

//        // Assert
//        Assert.True(result.Succeeded);
//        var user = result.Entity as User;
//        Assert.NotNull(user);
//        Assert.Equal(userDto.FirstName, user.FirstName);
//        Assert.Equal(userDto.LastName, user.LastName);
//        Assert.Equal(userDto.Email, user.Email);
//        Assert.Equal(0, user.Balance);
//        Assert.Equal("HashedPassword", user.Password);
//        Assert.Equal(userRole, user.Role);
//    }

//   
//}
