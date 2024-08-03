using Microsoft.AspNetCore.Identity;
using Moq;
using System.Text;

namespace Test.Mocks;

public static class UserManagerMock
{
    public static Mock<UserManager<TUser>> MockUserManager<TUser>() where TUser : class
    {
        Mock<IUserStore<TUser>> store = new Mock<IUserStore<TUser>>();
        Mock<UserManager<TUser>> mgr = new Mock<UserManager<TUser>>(store.Object, null, null, null, null, null, null, null, null);
        mgr.Object.UserValidators.Add(new UserValidator<TUser>());
        mgr.Object.PasswordValidators.Add(new PasswordValidator<TUser>());
        return mgr;
    }

    public static Mock<RoleManager<TRole>> MockRoleManager<TRole>() where TRole : class
    {
        IRoleStore<TRole> store = new Mock<IRoleStore<TRole>>().Object;
        List<IRoleValidator<TRole>> roles = [new RoleValidator<TRole>()];
        return new Mock<RoleManager<TRole>>(store, roles, MockLookupNormalizer(),
            new IdentityErrorDescriber(), null);
    }


    private static ILookupNormalizer MockLookupNormalizer()
    {
        Func<string, string> normalizerFunc = new(i =>
        {
            if (i == null)
            {
                return null;
            }
            else
            {
                return Convert.ToBase64String(Encoding.UTF8.GetBytes(i)).ToUpperInvariant();
            }
        });
        Mock<ILookupNormalizer> lookupNormalizer = new Mock<ILookupNormalizer>();
        lookupNormalizer.Setup(i => i.NormalizeName(It.IsAny<string>())).Returns(normalizerFunc);
        lookupNormalizer.Setup(i => i.NormalizeEmail(It.IsAny<string>())).Returns(normalizerFunc);
        return lookupNormalizer.Object;
    }
}
