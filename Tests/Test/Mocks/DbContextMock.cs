using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;

namespace Test.Mocks;
public static class DbContextMock
{
    public static DbSet<T> GetQueryableMockDbSet<T>(List<T> sourceList) where T : class
    {
        Mock<DbSet<T>> mock = sourceList.AsQueryable().BuildMockDbSet();

        return mock.Object;
    }

}
