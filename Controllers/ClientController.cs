using HomeBankingMindHub.DTOs;
using HomeBankingMindHub.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HomeBankingMindHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly IClientRepository _clientRepository;

        public ClientController(IClientRepository clientRepository){
            this._clientRepository = clientRepository;
        }

        [HttpGet]
        public IActionResult GetClients() {
            try {
                var clients = _clientRepository.GetAllClients();
                var clientsDTO = clients.Select(c => new ClientDTO(c)).ToList();
                return Ok(clientsDTO);
            }
            catch (Exception e) {
                return BadRequest(e.Message);
            };
        }

        [HttpGet("{id}")]
        public IActionResult GetClientById(long id)
        {
            try {
                var client = _clientRepository.FindById(id);

                //Por si pongo un id que no existe
                if (client == null) {
                    return NotFound($"Client with ID {id} not found.");
                }

                var clientDTO = new ClientDTO(client);
                return Ok(clientDTO);
                }
            catch (Exception e){
                return BadRequest(e.Message);
            }   
        }

    }
}
