using Application.Interfaces;
using Application.Users.Queries;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Test.Data;
using Test.Mocks;

namespace Test.Users.Queries
{
    public class DoesUserExistQueryTest
    {
        private readonly Mock<IDataContext> _mockContext;
        private readonly List<User> _users;
        public DoesUserExistQueryTest()
        {
            _mockContext = new Mock<IDataContext>();
            _users = UserFaker.GenerateUsers();

            var DbUsers = DbContextMock.GetQueryableMockDbSet(_users);
            _mockContext.Setup(x => x.Users).Returns(DbUsers);
        }

        [Fact]
        public async Task Handle_ExistingUser_ReturnsTrue()
        {
            // Arrange
            long existingAccountNumber = _users[0].AccountNumber;

            var handler = new DoesUserExistQueryHandler(_mockContext.Object);
            var request = new DoesUserExistQuery(existingAccountNumber);

            // Act
            bool result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result);
        }

        [Theory]
        [InlineData(0000000000)]//0 account number
        [InlineData(12345)]// invalid length account
        [InlineData(0123456789)] // no account can have a 0 present
        public async Task Handle_NonExistingUser_ReturnsFalse(long accountNumber)
        {
            // Arrange
            var handler = new DoesUserExistQueryHandler(_mockContext.Object);
            var request = new DoesUserExistQuery(accountNumber);

            // Act
            bool result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.False(result);
        }
    }
}
