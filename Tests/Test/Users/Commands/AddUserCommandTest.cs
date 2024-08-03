using Application.Common.Models;
using Application.Interfaces;
using Application.Users.Commands;
using Domain.Entities;
using Domain.Enum;
using Microsoft.AspNetCore.Identity;
using Moq;
using Test.Data;
using Test.Mocks;

namespace Test.Users.Commands;
public class AddUserCommandHandlerTests
{
    private readonly Mock<ISecretHasherService> _mockSecretHasher;
    private readonly Mock<IEmailService> _mockEmailSender;
    private readonly Mock<UserManager<User>> _mockUserManager;
    private readonly Mock<RoleManager<IdentityRole>> _mockRoleManager;

    private readonly List<User> _users;
    public AddUserCommandHandlerTests()
    {
        _mockSecretHasher = new Mock<ISecretHasherService>();
        _mockEmailSender = new Mock<IEmailService>();

        _users = UserFaker.GenerateValidUsers();

        _mockUserManager = UserManagerMock.MockUserManager<User>();

        _mockUserManager.Setup(x => x.Users).Returns(DbContextMock.GetQueryableMockDbSet(_users));

        _mockSecretHasher.Setup(x => x.Hash(It.IsAny<string>())).Returns("12345678*a");
        _mockRoleManager = UserManagerMock.MockRoleManager<IdentityRole>();

    }

    [Fact]
    public async Task HandleValidUser_ShouldReturnSuccess_WhenUserCreatedSuccessfully()
    {
        // Arrange
        _mockUserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((User?)null);
        _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        AddUserCommandHandler commandHandler = new(
            _mockEmailSender.Object,
            _mockUserManager.Object,
            _mockRoleManager.Object,
            _mockSecretHasher.Object);

        AddUserCommand command = new()
        {
            User = UserFaker.GenerateValidDto(),
            UserRole = UserRole.Admin,
        };

        // Act
        Result result = await commandHandler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.Succeeded);
        Assert.NotNull(result.Entity); // Check if a user object is returned
        Assert.Contains("created successfully", result.Message);
    }

    [Fact]
    public async Task HandleInvalidUser_ShouldReturnFailure_WhenUserCreationFails()
    {
        // Arrange
        _mockUserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((User)null);
        _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Test error" }));

        AddUserCommandHandler commandHandler = new(
            _mockEmailSender.Object,
            _mockUserManager.Object,
            _mockRoleManager.Object,
            _mockSecretHasher.Object);

        AddUserCommand command = new()
        {
            User = UserFaker.GenerateInvalidDto(),
            UserRole = UserRole.Admin,
        };

        // Act
        Result result = await commandHandler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Null(result.Entity);
        Assert.Contains("creation failed", result.Message);
        Assert.False(result.Succeeded);

    }
    [Fact]
    public async Task HandleInvalidUser_ShouldReturnFailure_WhenUserAlreadyExists()
    {
        // Arrange
        _mockUserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(new User { Email = "test@example.com" });

        AddUserCommandHandler commandHandler = new(
            _mockEmailSender.Object,
            _mockUserManager.Object,
            _mockRoleManager.Object,
            _mockSecretHasher.Object);

        AddUserCommand addUserCommand = new()
        {
            User = UserFaker.GenerateValidDto(),
            UserRole = UserRole.Admin,
        };

        // Act
        Result result = await commandHandler.Handle(addUserCommand, CancellationToken.None);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal("User Already Exists", result.Message);
    }
}
