using HomeBankingMindHub.Models;

namespace HomeBankingMindHub.Repositories.Implementations
{
    public class LoanRepository : RepositoryBase<Loan>, ILoanRepository 
    {
        public LoanRepository(HomeBankingContext repositoryContext) : base(repositoryContext) { }

        public Loan FindById(long id)
        {
            return FindByCondition(loan => loan.Id == id)
                .FirstOrDefault();
        }

        public IEnumerable<Loan> getAll()
        {
            return FindAll()
                .ToList();
        }
    }
}
