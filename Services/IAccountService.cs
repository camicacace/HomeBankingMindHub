using HomeBankingMindHub.DTOs;
using HomeBankingMindHub.Models;

namespace HomeBankingMindHub.Servicies
{
    public interface IAccountService
    {
        public string GenerateAccountNumber();
        public string UniqueAccountNumber();
        public AccountDTO CreateAccountDTO(Account account);
        public IEnumerable<AccountDTO> CreateAccountsDTO(IEnumerable<Account> accounts);
        public Account AccountById(long id);
        public IEnumerable<Account> GetAccounts();
        public IEnumerable<Account> AccountsByClient(long idClient);
        public Account GetAccountByNumber(string accountNumber);
        public void SaveAccount(Account account);
    }
}