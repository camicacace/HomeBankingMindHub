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
                Response<IEnumerable<TransactionDTO>> response = _transactionService.GetTransactions();

                if (response.StatusCode != 200)
                    return StatusCode(response.StatusCode, response.Message);

                return StatusCode(response.StatusCode, response.Data);
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
            try
            {
                Response<TransactionDTO> response = _transactionService.GetById(id);
                if (response.StatusCode != 200)
                    return StatusCode(response.StatusCode, response.Message);

                return StatusCode(response.StatusCode, response.Data);

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        [HttpPost]
        [Authorize(Policy = "ClientOnly")]

        public IActionResult PostTransaction([FromBody] TransferDTO newTransferDTO)
        {
            try
            {
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                
                Response<TransactionDTO> response = _transactionService.PostTransaction(email, newTransferDTO);

                if (response.StatusCode != 201)
                    return StatusCode(response.StatusCode, response.Message);

                return StatusCode(response.StatusCode, response.Data);

            }
            catch (Exception e) 
            {
                return StatusCode(500, e.Message);
            }
        }

    }
}
