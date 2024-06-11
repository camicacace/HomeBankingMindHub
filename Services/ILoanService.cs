using HomeBankingMindHub.DTOs;
using HomeBankingMindHub.Models;

namespace HomeBankingMindHub.Services
{
    public interface ILoanService
    {
        public Response<IEnumerable<LoanDTO>> GetLoans(string email);
        public Response<ClientLoanDTO> PostClientLoan(string email, LoanApplicationDTO loanApplicationDTO);

    }
}
