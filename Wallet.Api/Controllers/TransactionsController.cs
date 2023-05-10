using Wallet.Models;
using Wallet.Services;
using Microsoft.AspNetCore.Mvc;
using Wallet.Filters;
using Wallet.Api.Services;

namespace Wallet.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionsController : ControllerBase
{
    private readonly ITransactionsService _transactionService;
    private readonly ISendFundsService _sendFundsService;

    public TransactionsController(ITransactionsService transactionService, ISendFundsService sendFundsService)
    {
        _transactionService = transactionService;
        _sendFundsService = sendFundsService;
    }

    [HttpGet("{id:length(24)}")]
    public async Task<ActionResult<Transaction>> Get(string id)
    {
        var transaction = await _transactionService.Get(id);

        if (transaction is null)
        {
            return NotFound();
        }

        return transaction;
    }

    [HttpPost("create")]
    [TransActionFilter]
    public async Task<IActionResult> Post(Transaction transaction)
    {
        await _transactionService.CreateAsync(transaction);

        _ = _sendFundsService.sendTransaction(transaction);

        return CreatedAtAction(nameof(Get), new { id = transaction.Id }, transaction);
    }

    [HttpPut("{id:length(24)}")]
    public async Task<IActionResult> Update(string id, Transaction updatedTransaction)
    {
        var transaction = await _transactionService.Get(id);

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
        var transaction = await _transactionService.Get(id);

        if (transaction is null)
        {
            return NotFound();
        }

        await _transactionService.RemoveAsync(id);

        return NoContent();
    }
}
