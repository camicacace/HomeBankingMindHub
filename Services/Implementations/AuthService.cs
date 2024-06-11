using HomeBankingMindHub.DTOs;
using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
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

            var response = new Response<ClaimsIdentity>();

            if (String.IsNullOrEmpty(loginDTO.Email) || String.IsNullOrEmpty(loginDTO.Password))
            {
                response.StatusCode = 403;
                response.Message = "Missing fields";
            } else { 

                Client user = _clientRepository.FindByEmail(loginDTO.Email);
                if (user == null)
                {
                    response.StatusCode = 403;
                    response.Message = "Invalid user information";
                }
                else
                {
                    // hasheo la password ingresada para comparar con la almacenada
                    var result = _passwordHasher.VerifyHashedPassword(user, user.Password, loginDTO.Password);
                    if (result != PasswordVerificationResult.Success)
                    {
                        response.StatusCode = 403;
                        response.Message = "Invalid user information";
                    }
                    else
                    {
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

                        response.StatusCode = 200;
                        response.Data = claimsIdentity;
                        response.Message = "Autentication complete";

                    }
                }
            }
            return response;
        }
    }
}
