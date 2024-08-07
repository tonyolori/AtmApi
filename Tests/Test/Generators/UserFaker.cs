using Domain.Entities;
using Domain.Enum;


namespace Test.Data;


public static class UserFaker
{
    public static User GenerateValidUser()
    {
        return new User
        {
            Id = Guid.NewGuid().ToString(),
            AccountNumber = 1234567891,
            UserName = "johndoe",
            Email = "johndoe@example.com",
            PasswordHash = "dummypasswordhash1*", 
            Pin = "hashedPin",
            FirstName = "John",
            LastName = "Doe",
            Balance = 10000,
            Status = Status.Active,
            StatusDesc = Status.Active.ToString(),
        };
    }
    public static User GenerateInvalidUser()
    {
        return new User
        {
            Id = "11",
            AccountNumber = 12,
            Email = "ads",
            PasswordHash = "", // Replace with a secure hashing algorithm
            Pin = "hashedPin",
            FirstName = "John",
            LastName = "John",
            Status = Status.Active,
            StatusDesc = Status.Active.ToString(),
        };
    }
    public static List<User> GenerateValidUsers()
    {
        List<User> users =
        [
            new ()
        {
            Id = Guid.NewGuid().ToString(),
            AccountNumber = 1234567891,
            UserName = "johndoe",
            Email = "johndoe@example.com",
            PasswordHash = "dummypasswordhash", // Replace with a secure hashing algorithm
            Pin = "hashedPin",
            FirstName = "John",
            LastName = "Doe",
            Balance = 10000,
            Status = Status.Active,
            StatusDesc = Status.Active.ToString(),
        },
            new ()
        {
            Id = Guid.NewGuid().ToString(),
            AccountNumber = 1234567892,
            UserName = "alinSmith",
            Email = "alinSmith@example.com",
            PasswordHash = "dummypasswordhash",
            Pin = "hashedPin",
            FirstName = "alin",
            LastName = "Smith",
            Balance = 12000,
            Status = Status.Active,
            StatusDesc = Status.Active.ToString(),
        },
            new ()
        {
            Id = Guid.NewGuid().ToString(),
            AccountNumber = 1234567893,
            UserName = "seldoWellon",
            Email = "seldoWellon@example.com",
            PasswordHash = "dummypasswordhash",
            Pin = "hashedPin",
            FirstName = "seldo",
            LastName = "wellon",
            Balance = 10000,
            Status = Status.Active,
            StatusDesc = Status.Active.ToString(),
        },
        ];

        return users;


    }
    public static UserDto GenerateValidDto()
    {
        return new UserDto
        {
            Email = "johndoe@example.com",
            Password = "12345678*", // passes the validation algo
            Pin = 1234,
            FirstName = "John",
            LastName = "Doe"
        };
    }

    public static UserDto GenerateInvalidDto()
    {
        return new UserDto
        {
            Email = "johndoe", //invalid email format
            Password = "dummypassword", //will not pass validation algo
            Pin = 34, //too short
            FirstName = "John",
            LastName = "john" //name and surname cannot be the same
        };

    }

}

