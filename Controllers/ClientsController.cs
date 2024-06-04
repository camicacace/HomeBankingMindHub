using HomeBankingMindHub.DTOs;
using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace HomeBankingMindHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly IClientRepository _clientRepository;

        public ClientsController(IClientRepository clientRepository)
        {
            this._clientRepository = clientRepository;
        }

        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult GetClients() {
            try 
            {
                var clients = _clientRepository.GetAllClients();
                var clientsDTO = clients.Select(c => new ClientDTO(c)).ToList();
                return Ok(clientsDTO);
            }
            catch (Exception e) 
            {
                return BadRequest(e.Message);
            };
        }

        [HttpGet("{id}")]
        public IActionResult GetClientById(long id)
        {
            try {
                var client = _clientRepository.FindById(id);

                //Por si pongo un id que no existe
                if (client == null) 
                {
                    return NotFound($"Client with ID {id} not found.");
                }

                var clientDTO = new ClientDTO(client);
                return Ok(clientDTO);
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
                    return StatusCode(403,"User not found");
                }

                Client client = _clientRepository.FindByEmail(email);

                if (client == null)
                {
                    return StatusCode(403,"User not found");
                }

                var clientDTO = new ClientDTO(client);
                return Ok(clientDTO);

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

                Client user = _clientRepository.FindByEmail(newClientDTO.Email);
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

                    _clientRepository.Save(newClient);
                    return Created("", newClient);
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}
