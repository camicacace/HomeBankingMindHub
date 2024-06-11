using HomeBankingMindHub.DTOs;
using HomeBankingMindHub.Models;
using HomeBankingMindHub.Models.Enums;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Identity.Client;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HomeBankingMindHub.Services
{
    public interface ITransactionService
    {
        public Response<IEnumerable<TransactionDTO>> GetTransactions();
        public Response<TransactionDTO> GetById(long id);
        public Response<TransactionDTO> PostTransaction(string email, TransferDTO newTransferDTO);

    }
}
