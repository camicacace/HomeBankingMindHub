using HomeBankingMindHub.Models;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace HomeBankingMindHub.Repositories
{
    public interface IClientLoanRepository
    {
        public void Save(ClientLoan clientLoan);
    }
}
