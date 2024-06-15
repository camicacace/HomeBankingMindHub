using HomeBankingMindHub.DTOs;
using HomeBankingMindHub.Models;

namespace HomeBankingMindHub.Services
{
    public interface IClientService
    {

        public Response<IEnumerable<ClientDTO>> GetAllClients();
        public Response<ClientDTO> GetById(long id);
        public Response<ClientDTO> GetByEmail(string email);
        public Response<IEnumerable<AccountDTO>> GetAccounts(string email);
        public Response<ClientDTO> PostClient(NewClientDTO newClientDTO);
        public Response<AccountDTO> PostAccount(string email);
        public Response<CardDTO> PostCard(NewCardDTO newCardDTO, string email);

    }
}
