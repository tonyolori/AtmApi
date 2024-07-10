
using Domain.Models;

namespace Application.Helpers
{
    public static class UserExtensions
    {
        public static bool MatchPassword(this User user, string password)
        {
            return password == user.Password;
        }
        public static bool MatchPin(this User user, int pin)
        {
            return pin == user.Pin;
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

        public static void ChangePin(this User user, int newPin)
        {
            user.Pin = newPin;
            return;
        }
    }
}
