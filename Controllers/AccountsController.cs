using HomeBankingMindHub.Servicies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace HomeBankingMindHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountsController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet]
        [Authorize(Policy = "ClientOnly")]
        public IActionResult GetAccounts()
        {
            try
            {
                var accounts = _accountService.GetAccounts();
                return Ok(_accountService.CreateAccountsDTO(accounts));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("{id}")]
        [Authorize (Policy = "ClientOnly")]
        public IActionResult GetAccountById(long id)
        {
            try
            {
                var account = _accountService.AccountById(id);

                if (_accountService.AccountById(id) == null)
                {
                    return NotFound($"Account with ID {id} not found.");
                }

                return Ok(_accountService.CreateAccountDTO(account));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
