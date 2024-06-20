
using Domain.Models;

namespace Domain.Logic
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

        public static void Withdraw(this User user, uint amount)
        {
            if (user.Balance - amount >= 0)
            {
                user.Balance -= amount;
            }
            else
            {
                throw new InvalidOperationException("Insufficient funds");
            }
        }

        public static void Deposit(this User user, uint amount)
        {
            user.Balance += amount;
            return;
        }
    }
}
