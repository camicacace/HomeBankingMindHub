using HomeBankingMindHub.DTOs;
using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories;

namespace HomeBankingMindHub.Services.Implementations
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;

        public TransactionService(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        public TransactionDTO CreateDTO(Transaction transaction)
        {
            return new TransactionDTO(transaction);
        }

        public IEnumerable<TransactionDTO> CreateTransactionsDTO(IEnumerable<Transaction> transactions) 
        {
            return transactions.Select(t => new TransactionDTO(t));
        }

        public Transaction GetById(long id)
        {
            return _transactionRepository.FindById(id);
        }

        public IEnumerable<Transaction> GetTransactions()
        {
            return _transactionRepository.GetAllTransactions();
        }

        public void SaveTransaction(Transaction transaction)
        {
            _transactionRepository.Save(transaction);
        }
    }
}
