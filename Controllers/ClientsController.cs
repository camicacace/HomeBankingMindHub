using HomeBankingMindHub.DTOs;
using HomeBankingMindHub.Models;
using HomeBankingMindHub.Models.Enums;
using HomeBankingMindHub.Repositories;
using HomeBankingMindHub.Repositories.Implementations;
using HomeBankingMindHub.Services;
using HomeBankingMindHub.Servicies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;
using System;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using System.Security.Principal;

namespace HomeBankingMindHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly IClientService _clientService;
        private readonly IAccountService _accountService;
        private readonly ICardService _cardService;

        public ClientsController(IClientService clientService, IAccountService accountService, ICardService cardService)
        {
            _clientService = clientService;
            _accountService = accountService;
            _cardService = cardService;
        }

        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult GetClients() {
            try 
            {
                var clients = _clientService.GetAllClients();
                return Ok(_clientService.CreateClientsDTO(clients));
            }
            catch (Exception e) 
            {
                return BadRequest(e.Message);
            };
        }

        [HttpGet("{id}")]
        [Authorize (Policy = "ClientOnly")]
        public IActionResult GetClientById(long id)
        {
            try {
                var client = _clientService.GetById(id);

                //Por si pongo un id que no existe
                if (client == null) 
                {
                    return NotFound($"El cliente con el ID {id} no se encontro.");
                }

                return Ok(_clientService.CreateClientDTO(client));
                }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }   
        }

        [HttpGet("current")]
        [Authorize(Policy = "ClientOnly")]

        public IActionResult getCurrent()
        {
            try
            {
                // Estamos buscando en la cookie un elemento que tenga ese claim
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                if (email.IsNullOrEmpty())
                {
                    return StatusCode(403,"Usuario no encontrado");
                }

                Client client = _clientService.GetByEmail(email);

                if (client == null)
                {
                    return StatusCode(403, "Usuario no encontrado");
                }

                return Ok(_clientService.CreateClientDTO(client));

            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpGet("current/accounts")]
        [Authorize(Policy = "ClientOnly")]
        public IActionResult getCurrentAccounts()
        {
            try
            {
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                if (email.IsNullOrEmpty())
                {
                    return StatusCode(403, "Usuario no encontrado");
                }

                Client client = _clientService.GetByEmail(email);

                if (client == null)
                {
                    return StatusCode(403, "Usuario no encontrado");
                }

                var accounts = _accountService.AccountsByClient(client.Id);
                return Ok(_accountService.CreateAccountsDTO(accounts));

            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpPost]

        public IActionResult Post([FromBody] NewClientDTO newClientDTO)
        {
            try
            {
                if (String.IsNullOrEmpty(newClientDTO.Email) 
                    || String.IsNullOrEmpty(newClientDTO.Password) 
                    || String.IsNullOrEmpty(newClientDTO.FirstName) 
                    || String.IsNullOrEmpty(newClientDTO.LastName))
                    return StatusCode(403, "Faltan datos");

                Client user = _clientService.GetByEmail(newClientDTO.Email);
                if (user != null)
                {
                    return StatusCode(403, "Email está en uso");
                }
                else
                {
                    Client newClient = new Client
                    {
                        Email = newClientDTO.Email,
                        Password = newClientDTO.Password,
                        FirstName = newClientDTO.FirstName,
                        LastName = newClientDTO.LastName,
                    };

                    _clientService.SaveClient(newClient);
                    Client createdClient = _clientService.GetByEmail(newClient.Email);


                    Account newAccount = new Account
                    {
                        CreationDate = DateTime.Now,
                        Balance = 0,
                        ClientId = createdClient.Id,
                        Number = _accountService.UniqueAccountNumber(),

                    };

                    _accountService.SaveAccount(newAccount);
                    return Created();
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }


        [HttpPost("current/accounts")]
        [Authorize (Policy = "ClientOnly")]

        public IActionResult PostAccount()
        {

            try
            {
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                if (email.IsNullOrEmpty())
                {
                    return StatusCode(403, "Usuario no encontrado");
                }

                Client client = _clientService.GetByEmail(email);
                if (client == null)
                {
                    return StatusCode(403, "Usuario no encontrado");
                }

                var accounts = _accountService.AccountsByClient(client.Id);
                if (accounts.Count() >= 3)
                {
                    return StatusCode(403, "No se permiten mas de 3 cuentas por cliente");
                }
                else
                { 

                    Account newAccount = new Account
                    {
                        CreationDate = DateTime.Now,
                        Balance = 0,
                        ClientId = client.Id,
                        Number = _accountService.UniqueAccountNumber(),
                    };

                    _accountService.SaveAccount(newAccount);
                    return Created();

                }

            } catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }

        }

        [HttpPost("current/cards")]
        [Authorize(Policy = "ClientOnly")]

        public IActionResult PostCard([FromBody] NewCardDTO newCardDTO)
        {
            try
            {
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                if (email.IsNullOrEmpty())
                {
                    return StatusCode(403, "Usuario no encontrado");
                }

                Client client = _clientService.GetByEmail(email);
                if (client == null)
                {
                    return StatusCode(403, "Usuario no encontrado");
                }

                var clientCards = _cardService.getClientsCards(client.Id);
                if (clientCards.Count() >= 6 )
                {
                    return StatusCode(403, "No se permiten mas de 6 tarjetas por cliente");
                }
                else
                {
                    if (_cardService.getCardsByType(client.Id,newCardDTO.Type).Count() >= 3)
                    {
                        return StatusCode(403, $"No se permiten más de 3 tarjetas de tipo {newCardDTO.Type}");
                    }

                    if (_cardService.CardExists(client.Id,newCardDTO.Type,newCardDTO.Color))
                    {
                        return StatusCode(403, $"Ya existe una tarjeta tipo {newCardDTO.Type}, color {newCardDTO.Color}");
                    }            

                   string cardNumber = _cardService.UniqueCardNumber(newCardDTO.Type);


                    Card newCard = new Card
                    {
                        ClientId = client.Id,
                        CardHolder = client.FirstName + " " + client.LastName,
                        Type = newCardDTO.Type,
                        Color = newCardDTO.Color,
                        FromDate = DateTime.Now,
                        ThruDate = DateTime.Now.AddYears(5),
                        Number = cardNumber,
                        CVV = _cardService.GenerateCVV(),
                    };

                    _cardService.SaveCard(newCard);
                    return Created();
                }


            } catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}
