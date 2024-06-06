using HomeBankingMindHub.Controllers;
using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories;
using HomeBankingMindHub.Repositories.Implementations;
using System.Security.Cryptography;

namespace HomeBankingMindHub.Servicies.Implementations
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        public AccountService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public string GenerateAccountNumber()
        {
            Random random = new Random();
            int randomInt = random.Next(0, 100000000);
            // Para que tenga 8 digitos y pueda incluir 00000000
            string randomEightDigitNumber = randomInt.ToString("D8");
            return "VIN" + randomEightDigitNumber;
        }

        public string UniqueAccountNumber()
        {
            IEnumerable<Account> accounts = _accountRepository.GetAllAccounts();
            string randomNumber;

            do
            {
                randomNumber = GenerateAccountNumber();

            } while (accounts.Any(acc => acc.Number == randomNumber));

            return randomNumber;

        }

    }
}
