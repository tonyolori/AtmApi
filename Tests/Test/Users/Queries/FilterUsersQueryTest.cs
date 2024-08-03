using Application.Common.Models;
using Application.Users.Queries;
using Domain.Entities;
using Domain.Enum;
using Microsoft.AspNetCore.Identity;
using Moq;
using Test.Data;
using Test.Mocks;

namespace Test.Users.Queries;

public class FilterUsersQueryHandlerTest
{
    private readonly List<User> _users;
    private readonly Mock<UserManager<User>> _mockUserManager;

    public FilterUsersQueryHandlerTest()
    {
        _users = UserFaker.GenerateValidUsers();
        _mockUserManager = UserManagerMock.MockUserManager<User>();
        _mockUserManager.Setup(x => x.Users).Returns(DbContextMock.GetQueryableMockDbSet(_users));
    }

    [Fact]
    public async Task Handle_ReturnsAllUsers_WhenNoFiltersApplied()
    {
        // Arrange
        FilterUsersQuery request = new();
        FilterUsersQueryHandler handler = new(_mockUserManager.Object);

        // Act
        Result result = await handler.Handle(request, CancellationToken.None);
        List<User> userResults = (List<User>)result.Entity;

        // Assert
        _mockUserManager.Verify(x => x.Users, Times.Once);
        Assert.Equal(_users.Count, userResults.Count);
    }

    [Fact]
    public async Task Handle_FiltersByRole_ReturnsMatchingUsers()
    {
        // Arrange
        UserRole expectedRole = UserRole.Admin;
        _mockUserManager.Setup(x => x.GetRolesAsync(It.IsAny<User>()))
            .ReturnsAsync([expectedRole.ToString()]);

        List<User> usersWithRole = _users.Where(u => _mockUserManager.Object.GetRolesAsync(u).Result.Contains(expectedRole.ToString())).ToList();


        FilterUsersQueryHandler handler = new(_mockUserManager.Object);

        // Act
        Result result = await handler.Handle(new() { UserRole = (int)expectedRole }, CancellationToken.None);
        List<User> userResults = (List<User>)result.Entity;

        // Assert
        _mockUserManager.Verify(x => x.GetRolesAsync(It.IsAny<User>()), Times.AtLeastOnce);
        Assert.Equal(usersWithRole.Count, userResults.Count);
        Assert.True(userResults.All(u => _mockUserManager.Object.GetRolesAsync(u).Result.Contains(expectedRole.ToString())));
    }

    [Fact]
    public async Task Handle_FiltersByName_ReturnsMatchingUsers()
    {
        // Arrange
        string searchValue = "test";
        List<User> usersWithName = _users.Where(u => u.UserName.Contains(searchValue) || u.Email.Contains(searchValue)).ToList();
        FilterUsersQuery request = new FilterUsersQuery { SearchValue = searchValue };

        FilterUsersQueryHandler handler = new FilterUsersQueryHandler(_mockUserManager.Object);

        // Act
        Result result = await handler.Handle(request, CancellationToken.None);
        List<User> userResults = (List<User>)result.Entity;

        // Assert
        _mockUserManager.Verify(x => x.Users, Times.Once);
        Assert.Equal(usersWithName.Count, userResults.Count);
        Assert.True(userResults.All(u => u.UserName.Contains(searchValue) || u.Email.Contains(searchValue)));
    }

    [Fact]
    public async Task Handle_CombinesFilters_ReturnsMatchingUsers()
    {
        // Arrange
        UserRole expectedRole = UserRole.Admin;
        string searchValue = "test";
        _mockUserManager.Setup(x => x.GetRolesAsync(It.IsAny<User>()))
            .ReturnsAsync([expectedRole.ToString()]);

        List<User> usersWithRoleAndName = _users.Where(u =>
            _mockUserManager.Object.GetRolesAsync(u).Result.Contains(expectedRole.ToString()) &&
            (u.UserName.Contains(searchValue) || u.Email.Contains(searchValue))).ToList();



        FilterUsersQueryHandler handler = new FilterUsersQueryHandler(_mockUserManager.Object);

        // Act
        Result result = await handler.Handle(new() { UserRole = (int)expectedRole, SearchValue = searchValue }, CancellationToken.None);
        List<User> userResults = (List<User>)result.Entity;

        // Assert
        _mockUserManager.Verify(x => x.GetRolesAsync(It.IsAny<User>()), Times.AtLeastOnce);
        Assert.Equal(usersWithRoleAndName.Count, userResults.Count);

        Assert.True(userResults.All(u =>
            _mockUserManager.Object.GetRolesAsync(u).Result.Contains(expectedRole.ToString()) &&
            (u.UserName.Contains(searchValue) || u.Email.Contains(searchValue))));
    }

    [Fact]
    public async Task Handle_InvalidUserRole_ReturnsEmptyList()
    {
        // Arrange
        FilterUsersQuery request = new() { UserRole = -1 }; // Invalid user role
        FilterUsersQueryHandler handler = new(_mockUserManager.Object);

        // Act
        Result result = await handler.Handle(request, CancellationToken.None);
        List<User?> userResults = (List<User?>)result.Entity;

        // Assert
        _mockUserManager.Verify(x => x.GetRolesAsync(It.IsAny<User>()), Times.Never);
        Assert.Null(userResults);
        Assert.Contains("Invalid UserRole", result.Message);
    }

}