using HomeBankingMindHub.DTOs;
using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HomeBankingMindHub.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IClientRepository _clientRepository;
        private readonly PasswordHasher<Client> _passwordHasher;
        private readonly IConfiguration _configuration;

        public AuthService(IClientRepository clientRepository, IConfiguration configuration)
        {
            _clientRepository = clientRepository;
            _configuration = configuration;
            _passwordHasher = new PasswordHasher<Client>();
        }

        public Response<String> Login(LoginDTO loginDTO)
        {

            var response = new Response<String>();

            if (String.IsNullOrEmpty(loginDTO.Email) || String.IsNullOrEmpty(loginDTO.Password))
            {
                response.StatusCode = 403;
                response.Message = "Missing fields";
            }
            else
            {

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

                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

                        var token = new JwtSecurityToken(
                            claims: claims,
                            expires: DateTime.Now.AddMinutes(10),
                            signingCredentials: creds);

                        string createdToken = new JwtSecurityTokenHandler().WriteToken(token);

                        response.StatusCode = 200;
                        response.Data = createdToken;
                        response.Message = "Autentication complete";

                    }
                }
            }
            return response;
        }
    }
}