using HomeBankingMindHub.DTOs;
using HomeBankingMindHub.Models;
using HomeBankingMindHub.Models.Enums;
using HomeBankingMindHub.Repositories;
using HomeBankingMindHub.Repositories.Implementations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.IdentityModel.Tokens;
using System.Drawing.Printing;
using System.Linq;

namespace HomeBankingMindHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoansController : ControllerBase
    {
        private readonly IClientRepository _clientRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IClientLoanRepository _clientLoanRepository;
        private readonly ILoanRepository _loanRepository;

        public LoansController(IClientRepository clientRepository, IAccountRepository accountRepository, ITransactionRepository transactionRepository, IClientLoanRepository clientLoanRepository, ILoanRepository loanRepository)
        {
            _clientRepository = clientRepository;
            _accountRepository = accountRepository;
            _transactionRepository = transactionRepository;
            _clientLoanRepository = clientLoanRepository;
            _loanRepository = loanRepository;
        }

        [HttpGet]
        [Authorize(Policy = "ClientOnly")]

        public IActionResult GetLoans()
        {
            try
            {
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                if (email.IsNullOrEmpty())
                {
                    return StatusCode(403, "Usuario no encontrado");
                }

                Client client = _clientRepository.FindByEmail(email);

                if (client == null)
                {
                    return StatusCode(403, "Usuario no encontrado");
                }

                var loans = _loanRepository.getAll();
                var loansDTO = loans.Select(l => new LoanDTO(l)).ToList();
                return Ok(loansDTO);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpPost]
        [Authorize(Policy = "ClientOnly")]

        public IActionResult postLoans(LoanApplicationDTO loanApplicationDTO)
        {
            try
            {
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                if (email.IsNullOrEmpty())
                {
                    return StatusCode(403, "Usuario no encontrado");
                }

                Client client = _clientRepository.FindByEmail(email);

                if (client == null)
                {
                    return StatusCode(403, "Usuario no encontrado");
                }

                if (String.IsNullOrEmpty(loanApplicationDTO.ToAccountNumber) ||
                    String.IsNullOrEmpty(loanApplicationDTO.Payments) ||
                    loanApplicationDTO.Amount <= 0)
                {
                    return StatusCode(403, "Faltan datos");
                }

                var loan = _loanRepository.FindById(loanApplicationDTO.LoanId);
                if (loan == null)
                {
                    return StatusCode(403, "Prestamo no encontrado");
                }

                if (loanApplicationDTO.Amount > loan.MaxAmount)
                {
                    return StatusCode(403, "Monto invalido");
                }

                var allowedPayments = loan.Payments.Split(',').Select(int.Parse).ToList();
                if (!allowedPayments.Contains(Convert.ToInt32(loanApplicationDTO.Payments)))
                {
                    return StatusCode(403, "Numero de pagos invalidos");
                }

                var clientAccounts = _accountRepository.GetAccountsByClient(client.Id);
                if (!clientAccounts.Any(account => account.Number == loanApplicationDTO.ToAccountNumber))
                {
                    return StatusCode(403, "Cuenta incorrecta");
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
                return Created();
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

    }
}
