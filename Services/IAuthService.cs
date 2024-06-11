using HomeBankingMindHub.DTOs;
using HomeBankingMindHub.Models;
using System.Security.Claims;

namespace HomeBankingMindHub.Services
{
    public interface IAuthService
    {
        public Response<ClaimsIdentity> Login(LoginDTO loginDTO);
    }
}
