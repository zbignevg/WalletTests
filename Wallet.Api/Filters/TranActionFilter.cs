using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using Wallet.Models;
using Wallet.Services;

namespace Wallet.Filters
{
    public class TranActionFilter : ActionFilterAttribute, IActionFilter
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            Transaction transaction = (Transaction)context.ActionArguments.SingleOrDefault(p => p.Value is Transaction).Value;
            BankAccountService bankAccountService = context.HttpContext.RequestServices.GetService<BankAccountService>();

            if (!bankAccountService.ValidAndSufficientFunds(transaction.From, transaction.Amount))
            {
                throw new Exception("Insufficient funds");
            }
            
            base.OnActionExecuting(context);
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            base.OnActionExecuted(context);
        }
    }
}
