using HomeBankingMindHub.DTOs;
using HomeBankingMindHub.Models;
using HomeBankingMindHub.Models.Enums;
using HomeBankingMindHub.Repositories;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HomeBankingMindHub.Services.Implementations
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IAccountRepository _accountRepository;

        public TransactionService(ITransactionRepository transactionRepository, IClientRepository clientRepository, IAccountRepository accountRepository)
        {
            _transactionRepository = transactionRepository;
            _clientRepository = clientRepository;
            _accountRepository = accountRepository;
        }

        public Response<TransactionDTO> GetById(long id)
        {
            Transaction transaction= _transactionRepository.FindById(id);

            if (transaction == null)
            {
                return new Response<TransactionDTO>
                {
                    StatusCode = 403,
                    Message = $"Transaction with id {id} does not exist"
                };
            }
            
            var transactionDTO = new TransactionDTO(transaction);

            return new Response<TransactionDTO>
            {
                StatusCode = 200,
                Data = transactionDTO
            };
        }

        public Response<IEnumerable<TransactionDTO>> GetTransactions()
        {
            var transactions = _transactionRepository.GetAllTransactions();

            if (transactions == null){

                return new Response<IEnumerable<TransactionDTO>>
                {
                    StatusCode = 403,
                    Message = "No transactions"
                };
            }

            var transactionsDTO = transactions.Select(t => new TransactionDTO(t));

            return new Response<IEnumerable<TransactionDTO>>
            {
                StatusCode = 200,
                Data = transactionsDTO
            };

        }

        public Response<TransactionDTO> PostTransaction(string email, TransferDTO newTransferDTO)
        {
            Client client = _clientRepository.FindByEmail(email);

            if (client == null)
            {
                return new Response<TransactionDTO>
                {
                    StatusCode = 400,
                    Message = "User not found"
                };

            }

            if (newTransferDTO.FromAccountNumber.IsNullOrEmpty() ||
                    newTransferDTO.ToAccountNumber.IsNullOrEmpty() ||
                    newTransferDTO.Description.IsNullOrEmpty() ||
                    newTransferDTO.Amount <= 0)
            {
                return new Response<TransactionDTO>
                {
                    StatusCode = 403,
                    Message = "Missing fields"
                };

            }
            if (newTransferDTO.FromAccountNumber == newTransferDTO.ToAccountNumber)
            {
                return new Response<TransactionDTO>
                {
                    StatusCode = 403,
                    Message = "Origin and Destin account are the same"
                };
            }

            var originAccount = _accountRepository.FindByNumber(newTransferDTO.FromAccountNumber);
            if (originAccount.Balance < newTransferDTO.Amount)
            {
                return new Response<TransactionDTO>
                {
                    StatusCode = 403,
                    Message = "Balance lower than amount"
                };
            }

            var destinAccount = _accountRepository.FindByNumber(newTransferDTO.ToAccountNumber);
            if (destinAccount == null)
            {
                return new Response<TransactionDTO>
                {
                    StatusCode = 403,
                    Message = "Destin account does not exist"
                };
            }

            Transaction originTransaction = new Transaction
            {
                AccountId = originAccount.Id,
                Type = TransactionType.DEBIT.ToString(),
                Amount = -newTransferDTO.Amount,
                Description = newTransferDTO.Description + " - " + destinAccount.Number,
                Date = DateTime.Now,
            };

            Transaction destinTransaction = new Transaction
            {
                AccountId = destinAccount.Id,
                Type = TransactionType.CREDIT.ToString(),
                Amount = newTransferDTO.Amount,
                Description = newTransferDTO.Description + " - " + originAccount.Number,
                Date = DateTime.Now,
            };


            originAccount.Balance = originAccount.Balance - newTransferDTO.Amount;
            destinAccount.Balance = destinAccount.Balance + newTransferDTO.Amount;

            _transactionRepository.Save(originTransaction);
            _transactionRepository.Save(destinTransaction);
            _accountRepository.Save(originAccount);
            _accountRepository.Save(destinAccount);

            var transactionDTO = new TransactionDTO(originTransaction);



            return new Response<TransactionDTO>
            {
                StatusCode = 201,
                Message = "Transaction created",
                Data = transactionDTO
            };

        }

    }
}
