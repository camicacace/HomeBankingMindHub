using HomeBankingMindHub.DTOs;
using HomeBankingMindHub.Models;
using Microsoft.EntityFrameworkCore.Update.Internal;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HomeBankingMindHub.Servicies
{
    public interface IAccountService
    {
        public Response<AccountDTO> AccountById(long id);
        public Response<IEnumerable<AccountDTO>> GetAccounts();

    }
}