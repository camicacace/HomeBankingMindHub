using HomeBankingMindHub.DTOs;
using HomeBankingMindHub.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HomeBankingMindHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionRepository _transactionRepository;
        public TransactionsController(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        [HttpGet]

        public IActionResult GetTransactions()
        {
            try
            {
                var transactions = _transactionRepository.GetAllTransactions();
                var transactionsDTO = transactions.Select(t => new TransactionDTO(t)).ToList();
                return Ok(transactionsDTO);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("{id}")]

        public IActionResult GetById(int id)
        {
            var transaction = _transactionRepository.FindById(id);

            if (transaction == null)
            {
                return NotFound($"Transaction with ID {id} not found.");
            }

            try
            {
                var transactionDTO = new TransactionDTO(transaction);
                return Ok(transactionDTO);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
