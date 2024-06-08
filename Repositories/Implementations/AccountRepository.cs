using HomeBankingMindHub.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeBankingMindHub.Repositories.Implementations
{
    public class AccountRepository : RepositoryBase<Account>, IAccountRepository
    {
        public AccountRepository(HomeBankingContext repositoryContext) : base(repositoryContext) { }

        public Account FindById(long id)
        {
            return FindByCondition(account => account.Id == id).
                Include(account => account.Transactions).
                FirstOrDefault();              
        }

        public IEnumerable<Account> GetAllAccounts()
        {
            return FindAll().
                Include(account => account.Transactions).
                ToList();
        }

        public IEnumerable<Account> GetAccountsByClient(long clientId)
        {
            return FindByCondition(a => a.ClientId == clientId)
                .Include(a => a.Transactions)
                .ToList();
        }

        public Account FindByNumber(string number)
        {
            return FindByCondition(account => account.Number.ToUpper() == number.ToUpper())
                .Include(account => account.Transactions)
                .FirstOrDefault();
        }

        public void Save(Account account)
        {
            if (account.Id == 0)
            {
                Create(account);
            }
            else
            {
                Update(account);
            }

            SaveChanges();

            RepositoryContext.ChangeTracker.Clear();
        }
    }
}
