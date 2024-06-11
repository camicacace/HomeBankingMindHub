using HomeBankingMindHub.DTOs;
using HomeBankingMindHub.Models;
using HomeBankingMindHub.Models.Enums;
using HomeBankingMindHub.Services;
using HomeBankingMindHub.Services.Implementations;
using HomeBankingMindHub.Servicies;
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
        private readonly IClientService _clientService;
        private readonly IAccountService _accountService;
        private readonly ITransactionService _transactionService;
        private readonly ILoanService _loanService;

        public LoansController(IClientService clientService, IAccountService accountService, ITransactionService transactionService, ILoanService loanService)
        {
            _clientService = clientService;
            _accountService = accountService;
            _transactionService = transactionService;
            _loanService = loanService;
        }

        [HttpGet]
        [Authorize(Policy = "ClientOnly")]

        public IActionResult GetLoans()
        {
            try
            {
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                
                Response<IEnumerable<LoanDTO>> response = _loanService.GetLoans(email);

                if (response.StatusCode != 200)
                    return StatusCode(response.StatusCode, response.Message);

                return StatusCode(response.StatusCode, response.Data);

            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }


        [HttpPost]
        [Authorize(Policy = "ClientOnly")]

        public IActionResult PostLoans(LoanApplicationDTO loanApplicationDTO)
        {
            try
            {
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                Response<ClientLoanDTO> response = _loanService.PostClientLoan(email, loanApplicationDTO);

                if (response.StatusCode != 201)
                    return StatusCode(response.StatusCode, response.Message);

                return StatusCode(response.StatusCode, response.Message);

            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

    }
}
