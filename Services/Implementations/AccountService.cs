using HomeBankingMindHub.Controllers;
using HomeBankingMindHub.DTOs;
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

        public AccountDTO CreateAccountDTO(Account account)
        {
            return new AccountDTO(account);
        }


        public IEnumerable<AccountDTO> CreateAccountsDTO(IEnumerable<Account> accounts)
        {
            return accounts.Select(a => new AccountDTO(a));
        }

        public Account AccountById(long id)
        {
            return _accountRepository.FindById(id);
        }

        public IEnumerable<Account> GetAccounts()
        {
            return _accountRepository.GetAllAccounts();
        }

        public IEnumerable<Account> AccountsByClient(long idClient)
        {
            return _accountRepository.GetAccountsByClient(idClient).ToList();
        }

        public Account GetAccountByNumber(string accountNumber)
        {
            return _accountRepository.FindByNumber(accountNumber);
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

        public void SaveAccount(Account account)
        {
            _accountRepository.Save(account);
        }
    }
}
