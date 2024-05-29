using HomeBankingMindHub.Models;

namespace HomeBankingMindHub.Repositories
{
    public interface ITransactionRepository
    {
        IEnumerable<Transaction> GetAllTransactions();
        void Save(Transaction transaction);
        Transaction FindById(long id);
    }
}
