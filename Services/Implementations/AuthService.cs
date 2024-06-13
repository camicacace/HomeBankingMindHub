using HomeBankingMindHub.DTOs;
using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace HomeBankingMindHub.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IClientRepository _clientRepository;
        private readonly PasswordHasher<Client> _passwordHasher;

        public AuthService(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
            _passwordHasher = new PasswordHasher<Client>();
        }

        public Response<ClaimsIdentity> Login(LoginDTO loginDTO)
        {


            if (String.IsNullOrEmpty(loginDTO.Email) || String.IsNullOrEmpty(loginDTO.Password))
            {

                return new Response<ClaimsIdentity>
                {
                    StatusCode = 403,
                    Message = "Missing fields"
                };
                
            }

            Client user = _clientRepository.FindByEmail(loginDTO.Email);
            if (user == null)
            {
                return new Response<ClaimsIdentity>
                {
                    StatusCode = 403,
                    Message = "Invalid user information"
                };
            }

                    // hasheo la password ingresada para comparar con la almacenada
            var result = _passwordHasher.VerifyHashedPassword(user, user.Password, loginDTO.Password);
            if (result != PasswordVerificationResult.Success)
            {
                return new Response<ClaimsIdentity>
                {
                    StatusCode = 403,
                    Message = "Invalid user information"
                };
            }
            var claims = new List<Claim>
                        {
                            new Claim("Client", user.Email)
                        };

            if (user.Email.ToLower() == "ps@gmail.com")
            {
                claims.Add(new Claim("Admin", user.Email));
            }

            var claimsIdentity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme
            );

            return new Response<ClaimsIdentity>
            {
                StatusCode = 200,
                Data = claimsIdentity,
                Message = "Autentication complete"

            };
        }
    }
}
