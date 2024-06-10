using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories;

namespace HomeBankingMindHub.Services.Implementations
{
    public class ClientLoanService : IClientLoanService
    {
        private readonly IClientLoanRepository _clientLoanRepository;

        public ClientLoanService(IClientLoanRepository clientLoanRepository)
        {
            _clientLoanRepository = clientLoanRepository;
        }

        public void SaveClientLoan(ClientLoan clientLoan) 
        {
            _clientLoanRepository.Save(clientLoan);
        }
    }
}
