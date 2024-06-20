using Application.Interfaces;
using Domain.Models;

namespace Application.Helpers
{
    public class TransactionHelper(IUserRepository userRepository)
    {
        private readonly IUserRepository _userRepository = userRepository;
        public void Withdraw(User user, uint amount)
        {

            if (int.Parse(user.Balance.ToString()) - amount >= 0)
            {
                user.Balance -= amount;
                _userRepository.UpdateUserAsync(user);
            }
            else
            {
                throw new Exception("Insufficient balance");
            }
            _userRepository.UpdateUserAsync(user);


        }
        public void Deposit(User
            user, uint amount)
        {
            user.Balance += amount;
            _userRepository.UpdateUserAsync(user);

            return;
        }
        public async Task Transfer(User
            sendingAccount, User
            receivingAccount, uint amount)
        {
            try
            {
                sendingAccount.Withdraw(amount);
                receivingAccount.Deposit(amount);
            }
            
            catch(Exception e)
            {
                throw new Exception(e.Message);
            }


             _userRepository.UpdateUserAsync(sendingAccount);
             _userRepository.UpdateUserAsync(receivingAccount);
            
        }
        public static bool MatchPassword(User user, string password)
        {
            return password == user.Password;
        }

        public void ChangePin(User user, int newPin)
        {

            if (newPin.ToString().Length != 4)
            {
                throw new Exception("Pin length not equal to 4");
            }

            user.Pin = newPin;
            _userRepository.UpdateUserAsync(user);
        }
    }
}
