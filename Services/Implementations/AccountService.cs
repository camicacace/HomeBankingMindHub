using HomeBankingMindHub.Controllers;
using HomeBankingMindHub.DTOs;
using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories;
using HomeBankingMindHub.Repositories.Implementations;
using System.Net.Sockets;
using System.Security.Cryptography;

namespace HomeBankingMindHub.Servicies.Implementations
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        public AccountService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public Response<AccountDTO> AccountById(long id)
        {
            var account = _accountRepository.FindById(id);
            var response = new Response<AccountDTO>();

            if (account == null)
            {
                response.StatusCode = 404;
                response.Message = $"Account with ID {id} not found.";
            } else
            {
                var accountDTO = new AccountDTO(account);
                response.StatusCode = 200;
                response.Data = accountDTO;
            }

            return response;
        }

        public Response<IEnumerable<AccountDTO>> GetAccounts()
        {
            
            var accounts = _accountRepository.GetAllAccounts();
            var response = new Response<IEnumerable<AccountDTO>>();

            if (accounts == null)
            {
                response.StatusCode = 400;
                response.Message = "No se encontraron cuentas";
            } else
            {
                var accountsDTO = accounts.Select(a => new AccountDTO(a));
                response.StatusCode = 200;
                response.Message = "Ok";
                response.Data = accountsDTO;
            }

            return response;
        }

    }
}
