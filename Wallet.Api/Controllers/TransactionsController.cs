using Wallet.Models;
using Wallet.Services;
using Microsoft.AspNetCore.Mvc;
using Wallet.Filters;

namespace Wallet.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionsController : ControllerBase
{
    private readonly TransactionsService _transactionService;
    private readonly KafkaSendFundsService _kafkaSendFundsService;

    public TransactionsController(TransactionsService transactionService, KafkaSendFundsService kafkaSendFundsService)
    {
        _transactionService = transactionService;
        _kafkaSendFundsService = kafkaSendFundsService;
    }

    //[HttpPost("create")]
    //public async Task<IActionResult> Create(Transaction transaction)
    //{
    //    await _transactionService.CreateAsync(transaction);

    //    return CreatedAtAction(nameof(Get), new { id = transaction.Id }, transaction);
    //}


    [HttpGet("{id:length(24)}")]
    public async Task<ActionResult<Transaction>> Get(string id)
    {
        var transaction = await _transactionService.GetAsync(id);

        if (transaction is null)
        {
            return NotFound();
        }

        return transaction;
    }

    [HttpPost("create")]
    public async Task<IActionResult> Post(Transaction transaction)
    {
        await _transactionService.CreateAsync(transaction);

        _ = _kafkaSendFundsService.sendTransaction(transaction);

        return CreatedAtAction(nameof(Get), new { id = transaction.Id }, transaction);
    }

    [HttpPut("{id:length(24)}")]
    public async Task<IActionResult> Update(string id, Transaction updatedTransaction)
    {
        var transaction = await _transactionService.GetAsync(id);

        if (transaction is null)
        {
            return NotFound();
        }

        updatedTransaction.Id = transaction.Id;

        await _transactionService.UpdateAsync(id, updatedTransaction);

        return NoContent();
    }

    [HttpDelete("{id:length(24)}")]
    public async Task<IActionResult> Delete(string id)
    {
        var transaction = await _transactionService.GetAsync(id);

        if (transaction is null)
        {
            return NotFound();
        }

        await _transactionService.RemoveAsync(id);

        return NoContent();
    }
}
