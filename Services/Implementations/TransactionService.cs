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
            Response<TransactionDTO> response = new Response<TransactionDTO>();
            Transaction transaction= _transactionRepository.FindById(id);

            if (transaction == null)
            {
                response.StatusCode = 403;
                response.Message = $"Transaction with id {id} does not exist";
            }
            else
            {
                var transactionDTO = new TransactionDTO(transaction);
                response.StatusCode = 200;
                response.Data = transactionDTO;
            }

            return response;
        }

        public Response<IEnumerable<TransactionDTO>> GetTransactions()
        {
            Response<IEnumerable<TransactionDTO>> response = new Response<IEnumerable<TransactionDTO>>();
            var transactions = _transactionRepository.GetAllTransactions();

            if (transactions == null){
                response.StatusCode = 403;
                response.Message = "No transactions";
            }
            else
            {
                var transactionsDTO = transactions.Select(t => new TransactionDTO(t));
                response.StatusCode = 200;
                response.Data = transactionsDTO;
            }

            return response;
        }

        public Response<TransactionDTO> PostTransaction(string email, TransferDTO newTransferDTO)
        {
            Response<TransactionDTO> response = new Response<TransactionDTO>();
            Client client = _clientRepository.FindByEmail(email);

            if (client == null)
            {
                response.StatusCode = 403;
                response.Message = "Email in use";
            }
            else
            {
                if (newTransferDTO.FromAccountNumber.IsNullOrEmpty() ||
                    newTransferDTO.ToAccountNumber.IsNullOrEmpty() ||
                    newTransferDTO.Description.IsNullOrEmpty() ||
                    newTransferDTO.Amount <= 0)
                {
                    response.StatusCode = 403;
                    response.Message = "Missing fields";
                }
                else
                {


                    if (!client.Accounts.Any(a => a.Number.ToUpper() == newTransferDTO.FromAccountNumber.ToUpper()))
                    {
                        response.StatusCode = 403;
                        response.Message = "Origin account does not exist";
                    }
                    else
                    {

                        if (newTransferDTO.FromAccountNumber == newTransferDTO.ToAccountNumber)
                        {
                            response.StatusCode = 403;
                            response.Message = "Origin and Destin account are the same";
                        }
                        else
                        {

                            var originAccount = _accountRepository.FindByNumber(newTransferDTO.FromAccountNumber);
                            if (originAccount.Balance < newTransferDTO.Amount)
                            {
                                response.StatusCode = 403;
                                response.Message = "Balance lower than amount";
                            }
                            else
                            {

                                var destinAccount = _accountRepository.FindByNumber(newTransferDTO.ToAccountNumber);
                                if (destinAccount == null)
                                {
                                    response.StatusCode = 403;
                                    response.Message = "Destin account does not exist";
                                }
                                else
                                {

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
                                    response.StatusCode = 201;
                                    response.Message = "Transaction created";
                                    response.Data = transactionDTO;
                                }
                            }
                        }
                    }
                }
            }

            return response;
        }

    }
}
