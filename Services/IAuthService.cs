using HomeBankingMindHub.DTOs;
using HomeBankingMindHub.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace HomeBankingMindHub.Services
{
    public interface IAuthService
    {
        public Response<String> Login(LoginDTO loginDTO);
    }
}
