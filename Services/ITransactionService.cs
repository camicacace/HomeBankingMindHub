using HomeBankingMindHub.DTOs;
using HomeBankingMindHub.Models;

namespace HomeBankingMindHub.Services
{
    public interface ITransactionService
    {
        public IEnumerable<Transaction> GetTransactions();
        public Transaction GetById(long id);
        public TransactionDTO CreateDTO(Transaction transaction);
        public IEnumerable<TransactionDTO> CreateTransactionsDTO(IEnumerable<Transaction> transactions);
        public void SaveTransaction(Transaction transaction);
    }
}
