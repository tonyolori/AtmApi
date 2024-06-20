using Application.Interfaces;

namespace Application.Helpers
{
    public class AccountGeneratorHelper
    {
        private readonly IUserRepository _userRepository;

        public AccountGeneratorHelper(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<long> GenerateUniqueAccount()
        {
            long newId;
            bool exists;

            do
            {
                newId = GenerateRandomAccountNumber();
                exists = await _userRepository.DoesUserIdExistAsync(newId);
            } while (exists);

            return newId;
        }

        private long GenerateRandomAccountNumber()
        {
            var random = new Random();
            long accountId = 0;


            for (int i = 0; i < 10; i++)
            {
                accountId = accountId * 10 + random.Next(1, 10); // Generate a random digit (0-9) and add it to the account number
            }

            return accountId;
        }
    }

}
