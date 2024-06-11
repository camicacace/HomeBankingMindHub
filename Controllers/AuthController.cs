using HomeBankingMindHub.DTOs;
using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories;
using HomeBankingMindHub.Repositories.Implementations;
using HomeBankingMindHub.Services;
using HomeBankingMindHub.Services.Implementations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HomeBankingMindHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IClientRepository _clientRepository;
        private readonly PasswordHasher<Client> _passwordHasher;
        private readonly IConfiguration _configuration;

        public AuthController( IClientRepository clientRepository, IConfiguration configuration)
        {
            _clientRepository = clientRepository;
            _passwordHasher = new PasswordHasher<Client>();
            _configuration = configuration;
        }

        [HttpPost("login")]

        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            try
            {
                Client user = _clientRepository.FindByEmail(loginDTO.Email);
                if (user == null)
                {
                    return StatusCode(403, "Invalid user information");
                }

                // hasheo la password ingresada para comparar con la almacenada
                var result = _passwordHasher.VerifyHashedPassword(user, user.Password, loginDTO.Password);
                if (result != PasswordVerificationResult.Success)
                {
                    return StatusCode(403, "Invalid user information");
                }

                var claims = new List<Claim>
                {
                    new Claim("Client", user.Email)
                };

                if (user.Email.ToLower() == "cc@gmail.com")
                {
                    claims.Add(new Claim("Admin", user.Email));
                }

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

                var token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(10),
                    signingCredentials: creds);

                string createdToken = new JwtSecurityTokenHandler().WriteToken(token);

                return Ok(createdToken);

            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}
