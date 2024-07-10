using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Application.Transactions.Commands;
using Application.Transactions.Queries;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionRecordsController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpPost]
    public async Task<IActionResult> CreateTransaction(Transaction transaction)
    {
        var command = new AddTransactionCommand(transaction);
        await _mediator.Send(command);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTransaction(int id)
    {
        var command = new DeleteTransactionCommand(id);
        await _mediator.Send(command);
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetTransactions()
    {
        var query = new GetTransactionsQuery();
        var transactions = await _mediator.Send(query);
        return Ok(transactions);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTransaction(int id)
    {
        var query = new GetTransactionByIdQuery(id);
        var transaction = await _mediator.Send(query);
        if (transaction == null)
        {
            return NotFound();
        }
        return Ok(transaction);
    }
}
