using HomeBankingMindHub.DTOs;
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
                var response = _accountService.GetAccounts();

                if (response.StatusCode != 200)
                    return StatusCode(response.StatusCode, response.Message);
                else
                    return StatusCode(response.StatusCode, response.Data);

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
                var response = _accountService.AccountById(id);

                if (response.StatusCode != 200)
                    return StatusCode(response.StatusCode, response.Message);
                else
                    return StatusCode(response.StatusCode, response.Data);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
