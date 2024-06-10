using HomeBankingMindHub.DTOs;
using HomeBankingMindHub.Models;

namespace HomeBankingMindHub.Services
{
    public interface IClientService
    {
        public IEnumerable<Client> GetAllClients();
        public Client GetByEmail(string email);
        public Client GetById(long id);
        public IEnumerable<ClientDTO> CreateClientsDTO(IEnumerable<Client> clients);
        public ClientDTO CreateClientDTO(Client client);
        public void SaveClient(Client client);
    }
}
