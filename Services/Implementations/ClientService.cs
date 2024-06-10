using HomeBankingMindHub.DTOs;
using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using NuGet.Protocol;

namespace HomeBankingMindHub.Services.Implementations
{
    public class ClientService : IClientService
    {
        private readonly IClientRepository _clientRepository;

        public ClientService(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        public IEnumerable<Client> GetAllClients()
        {
            return _clientRepository.GetAllClients();
        }

        public Client GetByEmail(string email)
        {
            return _clientRepository.FindByEmail(email);
        }

        public Client GetById(long id)
        {
            return _clientRepository.FindById(id);
        }


        public IEnumerable<ClientDTO> CreateClientsDTO(IEnumerable<Client> clients)
        {
            return clients.Select(client => new ClientDTO(client));
        }
        public ClientDTO CreateClientDTO(Client client)
        {
            return new ClientDTO(client);
        }

        public void SaveClient(Client client)
        {
            _clientRepository.Save(client);
        }
    }
}
