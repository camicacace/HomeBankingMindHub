using HomeBankingMindHub.Models;

namespace HomeBankingMindHub.Services
{
    public interface ILoanService
    {
        public IEnumerable<Loan> GetLoans();
        public Loan GetLoanById(long id);
    }
}
