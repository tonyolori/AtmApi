using Bogus;
using Domain.Enum;
using Domain.Models;


namespace Test.Data;


public static class UserFaker
{
    public static User Generate()
    {
        var faker = new Faker<User>()
            .RuleFor(u => u.Id, f => f.IndexFaker) // Unique sequential IDs
            .RuleFor(u => u.AccountNumber, GetAccount)
            .RuleFor(u => u.Email, f => f.Internet.Email(f.Name.FirstName() + "." + f.Name.LastName() + "@example.com"))
            //.RuleFor(u => u.Password, f => f.Internet.Password() + "*") // Strong passwords for tests
            .RuleFor(u => u.Password, () => "12345678*a") // default password for tests
            .RuleFor(u => u.Pin, f => f.Random.Int(1000, 9999)) // 4-digit PIN
            .RuleFor(u => u.FirstName, f => f.Name.FirstName())
            .RuleFor(u => u.LastName, f => f.Name.LastName())
            .RuleFor(u => u.Balance, f => (long)f.Finance.Amount(0, 1000000)) // Random balance within range
            .RuleFor(u => u.Role, f => f.PickRandom(UserRole.User, UserRole.Admin)); // Random user role

        return faker.Generate();
    }
    
    public static List<User> GenerateUsers(int amount = 8)
    {
        var users = new List<User>();
        for (int i = 0; i < amount; i++)
        {
            users.Add(Generate()); // Call Generate twice per user one to create the Faker<User> and second to get the actual user

        }
        return users;
    }

    public static UserDto GenerateValidDto()
    {
        var faker = new Faker<UserDto>()
            .RuleFor(u => u.Email, f => f.Internet.Email(f.Name.FirstName() + "." + f.Name.LastName() + "@example.com"))
            .RuleFor(u => u.Password, f => "12345678*a") // Strong passwords for tests f.Internet.Password()
            .RuleFor(u => u.Pin, f => f.Random.Int(1000, 9999)) // 4-digit PIN
            .RuleFor(u => u.FirstName, f => f.Name.FirstName())
            .RuleFor(u => u.LastName, f => f.Name.LastName());

        return faker.Generate();
    }

    public static UserDto GenerateInvalidDto()
    {
        var faker = new Faker<UserDto>()
            .RuleFor(u => u.Email, f => f.Internet.Email(f.Name.FirstName() + "." + f.Name.LastName() + "@example.com"))
            .RuleFor(u => u.Password, f => "1234567") // Strong passwords for tests f.Internet.Password()
            .RuleFor(u => u.Pin, f => f.Random.Int(1000, 9999)) // 4-digit PIN
            .RuleFor(u => u.FirstName, () => "Mark")
            .RuleFor(u => u.LastName, () => "Mark");

        return faker.Generate();
    }


    private static long GetAccount(Faker f)
    {
        return long.Parse(f.Finance.Account(10));
    }

    public static UserRole GetRole()
    {
        return new Faker().PickRandom(UserRole.User, UserRole.Admin);
    }
}

