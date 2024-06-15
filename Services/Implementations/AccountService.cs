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

            if (account == null)
            {

                return new Response<AccountDTO>
                {
                    StatusCode = 404,
                    Message = $"Account with ID {id} not found."
                };
                
            } 
                var accountDTO = new AccountDTO(account);

            return new Response<AccountDTO>()
            {
                StatusCode = 200,
                Data = accountDTO
            };
        }

        public Response<IEnumerable<AccountDTO>> GetAccounts()
        {
            
            var accounts = _accountRepository.GetAllAccounts();

            if (accounts == null)
            {
                return new Response<IEnumerable<AccountDTO>>
                {
                    StatusCode = 400,
                    Message = "No accounts were found."
                };
            } 
                var accountsDTO = accounts.Select(a => new AccountDTO(a));

            return new Response<IEnumerable<AccountDTO>>
            {
                StatusCode = 200,
                Message = "Ok",
                Data = accountsDTO
            };
        }

    }
}
