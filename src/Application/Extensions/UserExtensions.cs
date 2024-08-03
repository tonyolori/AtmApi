
using Domain.Entities;

namespace Application.Extensions;
public static class UserExtensions
{
    public static bool MatchPin(this User user, string pin)
    {
        return user.Pin.Equals(pin);
    }

    public static long GetBalance(this User user)
    {
        return user.Balance;
    }

    public static bool Withdraw(this User user, uint amount)
    {
        if (user.Balance - amount >= 0)
        {
            user.Balance -= amount;
            return true;
        }
        return false;
    }

    public static void Deposit(this User user, uint amount)
    {
        user.Balance += amount;
        return;
    }

    public static void ChangePin(this User user, string newPin)
    {
        user.Pin = newPin;
        return;
    }
}
