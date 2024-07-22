using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;

namespace Test.Mocks;
public static class DbContextMock
{
    public static DbSet<T> GetQueryableMockDbSet<T>(List<T> sourceList) where T : class
    {
        var mock = sourceList.AsQueryable().BuildMockDbSet();
        
        return mock.Object;
    }

}

//var queryable = sourceList.AsQueryable();
//var dbSet = new Mock<DbSet<T>>();
//dbSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
//dbSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
//dbSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
//dbSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());
//queryable.Setup(d => d.Add(It.IsAny<T>())).Callback<T>((s) => sourceList.Add(s));