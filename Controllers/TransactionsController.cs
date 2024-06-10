using HomeBankingMindHub.DTOs;
using HomeBankingMindHub.Models;
using HomeBankingMindHub.Models.Enums;
using HomeBankingMindHub.Repositories;
using HomeBankingMindHub.Services;
using HomeBankingMindHub.Servicies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Linq;

namespace HomeBankingMindHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
        private readonly IAccountService _accountService;
        private readonly IClientService _clientService;
        public TransactionsController(ITransactionService transactionService, IAccountService accountService, IClientService clientService)
        {
            _transactionService = transactionService;
            _accountService = accountService;
            _clientService = clientService;
        }

        [HttpGet]
        [Authorize (Policy = "AdminOnly")]

        public IActionResult GetTransactions()
        {
            try
            {
                var transactions = _transactionService.GetTransactions();
                return Ok(_transactionService.CreateTransactionsDTO(transactions));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "AdminOnly")]

        public IActionResult GetById(int id)
        {
            var transaction = _transactionService.GetById(id);

            if (transaction == null)
            {
                return NotFound($"Transaction with ID {id} not found.");
            }

            try
            {
                return Ok(_transactionService.CreateDTO(transaction));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        [Authorize(Policy = "ClientOnly")]

        public IActionResult postTransaction([FromBody] TransferDTO newTransferDTO)
        {
            try
            {
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                if (email.IsNullOrEmpty())
                {
                    return StatusCode(403, "Usuario no encontrado");
                }

                Client client = _clientService.GetByEmail(email);
                if (client == null)
                {
                    return StatusCode(403, "Usuario no encontrado");
                }

                // Condiciones de la transferencia

                if (String.IsNullOrEmpty(newTransferDTO.FromAccountNumber) || 
                    String.IsNullOrEmpty(newTransferDTO.ToAccountNumber) ||
                    String.IsNullOrEmpty(newTransferDTO.Description) ||
                    newTransferDTO.Amount <= 0)
                    return StatusCode(403, "Faltan datos");

                if (!client.Accounts.Any(a => a.Number.ToUpper() == newTransferDTO.FromAccountNumber.ToUpper()))
                {
                    return StatusCode(403, "La cuenta origen no existe");
                }

                if (newTransferDTO.FromAccountNumber == newTransferDTO.ToAccountNumber)
                {
                    return StatusCode(403, "Cuenta origen y destino iguales");
                }

                var originAccount = _accountService.GetAccountByNumber(newTransferDTO.FromAccountNumber);
                if (originAccount.Balance < newTransferDTO.Amount)
                {
                    return StatusCode(403, "No hay suficiente saldo para hacer la transferencia");
                }

                var destinAccount = _accountService.GetAccountByNumber(newTransferDTO.ToAccountNumber);
                if (destinAccount == null)
                {
                    return StatusCode(403, "La cuenta destino no existe");
                }

                Transaction originTransaction = new Transaction
                {
                    AccountId = originAccount.Id,
                    Type = TransactionType.DEBIT.ToString(),
                    Amount = - newTransferDTO.Amount,
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

                _transactionService.SaveTransaction(originTransaction);
                _transactionService.SaveTransaction(destinTransaction);
                _accountService.SaveAccount(originAccount);
                _accountService.SaveAccount(destinAccount);
                return Created();

            }
            catch (Exception e) 
            {
                return StatusCode(500, e.Message);
            }
        }

    }
}
