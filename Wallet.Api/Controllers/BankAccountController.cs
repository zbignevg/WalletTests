using Wallet.Models;
using Wallet.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Wallet.Helpers;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection.Metadata;

namespace Wallet.Controllers;

//[Authorize]
[ApiController]
[Route("api/[controller]")]
public class BankAccountController : ControllerBase
{
    public IConfiguration _configuration;
    private readonly BankAccountService _bankAccountService;
    public BankAccountController(IConfiguration config, BankAccountService bankAccountService)
    {
        _bankAccountService = bankAccountService;
        _configuration = config;
    }

    //public class AddRequiredHeaderParameter : IOperationFilter { public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription) { if (operation.parameters == null) operation.parameters = new List<Parameter>(); operation.parameters.Add(new Parameter { name = "Foo-Header", @in = "header", type = "string", required = true }); } }
    //c.OperationFilter<AddRequiredHeaderParameter>();


    [HttpGet("{id:length(24)}")]
    public async Task<ActionResult<BankAccount>> Get(string id)
    {
        var bankAccount = await _bankAccountService.Get(id);

        if (bankAccount is null)
        {
            return NotFound();
        }

        return bankAccount;
    }

    [HttpPost]
    public async Task<IActionResult> Post(BankAccount bankAccount)
    {
        //var result = SecurityManager.VerifyJWT(_configuration, authToken);

        await _bankAccountService.Create(bankAccount);

        return CreatedAtAction(nameof(Get), new { id = bankAccount.Id }, bankAccount);
    }

    [HttpPut("{id:length(24)}")]
    public async Task<IActionResult> Update(string id, BankAccount updatedBankAccount)
    {
        var bankAccount = await _bankAccountService.Get(id);

        if (bankAccount is null)
        {
            return NotFound();
        }

        updatedBankAccount.Id = bankAccount.Id;

        await _bankAccountService.Update(id, updatedBankAccount);

        return NoContent();
    }

    [HttpDelete("{id:length(24)}")]
    public async Task<IActionResult> Delete(string id)
    {
        var bankAccount = await _bankAccountService.Get(id);

        if (bankAccount is null)
        {
            return NotFound();
        }

        await _bankAccountService.Remove(id);

        return NoContent();
    }
}
