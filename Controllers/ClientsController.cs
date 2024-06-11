using HomeBankingMindHub.DTOs;
using HomeBankingMindHub.Models;
using HomeBankingMindHub.Models.Enums;
using HomeBankingMindHub.Repositories;
using HomeBankingMindHub.Repositories.Implementations;
using HomeBankingMindHub.Services;
using HomeBankingMindHub.Servicies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;
using System;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;



namespace HomeBankingMindHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly IClientService _clientService;
        private readonly IAccountService _accountService;

        public ClientsController(IClientService clientService, IAccountService accountService)
        {
            _clientService = clientService;
            _accountService = accountService;
        }

        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult GetClients() {
            try 
            {
                Response<IEnumerable<ClientDTO>> response = _clientService.GetAllClients();

                if (response.StatusCode != 200)
                    return StatusCode(response.StatusCode, response.Message);

                return StatusCode(response.StatusCode, response.Data);
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
                
                Response<ClientDTO> response = _clientService.GetById(id);
                if (response.StatusCode != 200)
                    return StatusCode(response.StatusCode, response.Message);

                return StatusCode(response.StatusCode, response.Data);

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

                Response<ClientDTO> response = _clientService.GetByEmail(email);

                if (response.StatusCode != 200)
                    return StatusCode(response.StatusCode, response.Message);

                return StatusCode(response.StatusCode, response.Data);

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

                Response<IEnumerable<AccountDTO>> response = _clientService.GetAccounts(email);

                if (response.StatusCode != 200)
                    return StatusCode(response.StatusCode, response.Message);

                return StatusCode(response.StatusCode, response.Data);

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
                Response<ClientDTO> response = _clientService.PostClient(newClientDTO);

                if (response.StatusCode != 201)
                    return StatusCode(response.StatusCode, response.Message);

                return StatusCode(response.StatusCode, response.Message);

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
                
                Response<AccountDTO> response = _clientService.PostAccount(email);

                if (response.StatusCode != 201)
                    return StatusCode(response.StatusCode, response.Message);

                return StatusCode(response.StatusCode, response.Message);

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

                Response<CardDTO> response = _clientService.PostCard(newCardDTO, email);

                if (response.StatusCode != 201)
                    return StatusCode(response.StatusCode, response.Message);

                return StatusCode(response.StatusCode, response.Data);


            } catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}
