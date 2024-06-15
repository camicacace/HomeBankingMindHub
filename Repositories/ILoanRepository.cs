using HomeBankingMindHub.Models;

namespace HomeBankingMindHub.Repositories
{
    public interface ILoanRepository
    {
        public IEnumerable<Loan> getAll();
        public Loan FindById(long id);
    }
}
