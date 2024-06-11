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
using System.Security.Claims;

namespace HomeBankingMindHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IClientRepository _clientRepository;
        private readonly PasswordHasher<Client> _passwordHasher;

        public AuthController( IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
            _passwordHasher = new PasswordHasher<Client>();
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


                var claimsIdentity = new ClaimsIdentity(
                    claims,
                    CookieAuthenticationDefaults.AuthenticationScheme
                    );

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme, 
                    new ClaimsPrincipal(claimsIdentity));

                return Ok();

            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpPost("logout")]

        public async Task<IActionResult> Logout()
        {
            try
            {
                await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}
