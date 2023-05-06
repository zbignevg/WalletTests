using Microsoft.AspNetCore.Mvc.Filters;
using Wallet.Services;

namespace Wallet.Filters
{
    public class TranActionFilter : ActionFilterAttribute, IActionFilter
    {
        private readonly BankAccountService _bankAccountService;
        public TranActionFilter(BankAccountService bankAccountService) 
        { 
            _bankAccountService = bankAccountService;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var bb = context.ActionArguments;
            var transaction = context.ActionArguments.FirstOrDefault().Value;



            base.OnActionExecuting(context);
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            //base.OnActionExecuted(context);
        }
    }
}
