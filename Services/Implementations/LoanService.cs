using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories;

namespace HomeBankingMindHub.Services.Implementations
{
    public class LoanService : ILoanService
    {
        private readonly ILoanRepository _loanRepository;

        public LoanService(ILoanRepository loanRepository)
        {
            _loanRepository = loanRepository;
        }

        public Loan GetLoanById(long id)
        {
            return _loanRepository.FindById(id);
        }

        public IEnumerable<Loan> GetLoans()
        {
            return _loanRepository.getAll();
        }
    }
}
