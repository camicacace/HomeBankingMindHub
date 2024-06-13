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

            Client client = _clientRepository.FindByEmail(email);

            if (client == null)
            {
                return new Response<IEnumerable<LoanDTO>>
                {
                    StatusCode = 403,
                    Message = "User not found"
                };

            }
            var loans = _loanRepository.getAll();

            if (loans == null)
            {
                return new Response<IEnumerable<LoanDTO>>
                {
                    StatusCode = 403,
                    Message = "No loans"
                };
            }
            var loansDTO = loans.Select(l => new LoanDTO(l));

            return new Response<IEnumerable<LoanDTO>>
            {
                StatusCode = 200,
                Data = loansDTO
            };

        }

        public Response<ClientLoanDTO> PostClientLoan(string email, LoanApplicationDTO loanApplicationDTO)
        {
            Client client = _clientRepository.FindByEmail(email);

            if (client == null)
            {
                return new Response<ClientLoanDTO>
                {
                    StatusCode = 403,
                    Message = "User not found"
                };
            }

            if (String.IsNullOrEmpty(loanApplicationDTO.ToAccountNumber) ||
                String.IsNullOrEmpty(loanApplicationDTO.Payments))
            {
                return new Response<ClientLoanDTO>
                {
                    StatusCode = 403,
                    Message = "Missing fields"
                };
            }

            var loan = _loanRepository.FindById(loanApplicationDTO.LoanId);
            if (loan == null)
            {
                return new Response<ClientLoanDTO>
                {
                    StatusCode = 403,
                    Message = "Loan does not exist"
                };
            }

            if (loanApplicationDTO.Amount > loan.MaxAmount || loanApplicationDTO.Amount <= 0)
            {
                return new Response<ClientLoanDTO>
                {
                    StatusCode = 403,
                    Message = "Exceeds max amount"
                };
            }

            var allowedPayments = loan.Payments.Split(',').Select(int.Parse).ToList();
            if (!allowedPayments.Contains(Convert.ToInt32(loanApplicationDTO.Payments)))
            {
                return new Response<ClientLoanDTO>
                {
                    StatusCode = 403,
                    Message = "Invalid amount of payments"
                };
            }

            var clientAccounts = _accountRepository.GetAccountsByClient(client.Id).ToList();
            if (clientAccounts == null)
            {
                return new Response<ClientLoanDTO>
                {
                    StatusCode = 400,
                    Message = "No accounts"
                };
            }

            if (!clientAccounts.Any(account => account.Number == loanApplicationDTO.ToAccountNumber))
            {
                return new Response<ClientLoanDTO>
                {
                    StatusCode = 400,
                    Message = "Incorrect account"
                };
            }

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

            return new Response<ClientLoanDTO>
            {
                StatusCode = 201,
                Message = "Loan created"
            };
        }

    }
}