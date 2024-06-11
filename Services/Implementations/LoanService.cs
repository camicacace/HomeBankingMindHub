using HomeBankingMindHub.DTOs;
using HomeBankingMindHub.Models;
using HomeBankingMindHub.Models.Enums;
using HomeBankingMindHub.Repositories;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Collections.Generic;
using System.Net.Sockets;

namespace HomeBankingMindHub.Services.Implementations
{
    public class LoanService : ILoanService
    {
        private readonly ILoanRepository _loanRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IClientLoanRepository _clientLoanRepository;
        private readonly ITransactionRepository _transactionRepository;

        public LoanService(ILoanRepository loanRepository, IClientRepository clientRepository, IAccountRepository accountRepository, IClientLoanRepository clientLoanRepository, ITransactionRepository transactionRepository)
        {
            _loanRepository = loanRepository;
            _clientRepository = clientRepository;
            _accountRepository = accountRepository;
            _clientLoanRepository = clientLoanRepository;
            _transactionRepository = transactionRepository;
        }

        public Response<IEnumerable<LoanDTO>> GetLoans(string email)
        {
            Response<IEnumerable<LoanDTO>> response = new Response<IEnumerable<LoanDTO>>();
            Client client = _clientRepository.FindByEmail(email);

            if (client == null)
            {
                response.StatusCode = 403;
                response.Message = "User not found";
            }
            else
            {

                var loans = _loanRepository.getAll();

                if (loans == null)
                {
                    response.StatusCode = 403;
                    response.Message = "No loans";
                }
                else
                {
                    var loansDTO = loans.Select(l => new LoanDTO(l));
                    response.StatusCode = 200;
                    response.Data = loansDTO;
                }
            }
            return response;
        }

        public Response<ClientLoanDTO> PostClientLoan(string email, LoanApplicationDTO loanApplicationDTO)
        {
            Response<ClientLoanDTO> response = new Response<ClientLoanDTO>();
            Client client = _clientRepository.FindByEmail(email);

            if (client == null)
            {
                response.StatusCode = 403;
                response.Message = "User not found";
            }
            else
            {
                if (String.IsNullOrEmpty(loanApplicationDTO.ToAccountNumber) ||
                    String.IsNullOrEmpty(loanApplicationDTO.Payments))
                {
                    response.StatusCode = 403;
                    response.Message = "Missing fields";
                }
                else { 

                var loan = _loanRepository.FindById(loanApplicationDTO.LoanId);
                if (loan == null)
                {
                    response.StatusCode = 403;
                    response.Message = "Loan does not exist";
                }

                if (loanApplicationDTO.Amount > loan.MaxAmount || loanApplicationDTO.Amount <= 0)
                {
                    response.StatusCode = 403;
                    response.Message = "Excedes max amount";
                } else { 

                var allowedPayments = loan.Payments.Split(',').Select(int.Parse).ToList();
                    if (!allowedPayments.Contains(Convert.ToInt32(loanApplicationDTO.Payments)))
                    {
                        response.StatusCode = 403;
                        response.Message = "Invalid amount of payments";
                    }
                    else
                    {
                        var clientAccounts = _accountRepository.GetAccountsByClient(client.Id).ToList();
                            if (clientAccounts == null)
                            {
                                response.StatusCode = 400;
                                response.Message = "No accounts";
                            }
                            else
                            {

                                if (!clientAccounts.Any(account => account.Number == loanApplicationDTO.ToAccountNumber))
                                {
                                    response.StatusCode = 400;
                                    response.Message = "Incorrect account";
                                }
                                else
                                {

                                    ClientLoan clientLoan = new ClientLoan
                                    {
                                        Amount = loanApplicationDTO.Amount + 0.2 * loanApplicationDTO.Amount,
                                        Payments = loanApplicationDTO.Payments,
                                        ClientId = client.Id,
                                        LoanId = loanApplicationDTO.LoanId,
                                    };

                                    _clientLoanRepository.Save(clientLoan);

                                    var account = _accountRepository.FindByNumber(loanApplicationDTO.ToAccountNumber);

                                    Transaction transaction = new Transaction
                                    {
                                        AccountId = account.Id,
                                        Type = TransactionType.CREDIT.ToString(),
                                        Amount = loanApplicationDTO.Amount + 0.2 * loanApplicationDTO.Amount,
                                        Description = loan.Name + " - Loan approved",
                                        Date = DateTime.Now,
                                    };
                                    _transactionRepository.Save(transaction);

                                    account.Balance += loanApplicationDTO.Amount + 0.2 * loanApplicationDTO.Amount;
                                    _accountRepository.Save(account);

                                    response.StatusCode = 201;
                                    response.Message = "Loan created";
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
