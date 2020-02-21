using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Datify.API.Data;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace Datify.API.Helpers
{
    public class LogUserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Get action after execution
            var resultContext = await next();
            // Get user id from token
            var userId = int.Parse(resultContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            // Get instance of db
            var repo = resultContext.HttpContext.RequestServices.GetService<IDatingRepository>();
            // Get user using user id
            var user = await repo.GetUser(userId);
            // Update user last active date
            user.LastActive = DateTime.Now;
            await repo.SaveAll();
        }  
    }
}