using HomeBankingMindHub.DTOs;
using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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

        public IActionResult getCurrent()
        {
            try
            {
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                if (email == string.Empty)
                {
                    return Forbid();
                }

                Client client = _clientRepository.FindByEmail(email);

                if (client == null)
                {
                    return Forbid();
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

        public IActionResult Post([FromBody] Client client)
        {
            try
            {
                if (String.IsNullOrEmpty(client.Email) 
                    || String.IsNullOrEmpty(client.Password) 
                    || String.IsNullOrEmpty(client.FirstName) 
                    || String.IsNullOrEmpty(client.LastName))
                    return StatusCode(403, "datos inválidos");

                Client user = _clientRepository.FindByEmail(client.Email);
                if (user != null)
                {
                    return StatusCode(403, "Email está en uso");
                }
                else
                {
                    Client newClient = new Client
                    {
                        Email = client.Email,
                        Password = client.Password,
                        FirstName = client.FirstName,
                        LastName = client.LastName,
                    };

                    _clientRepository.Save(newClient);
                    var newClientDTO = new ClientDTO(newClient);
                    return Created("", newClientDTO);
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}
