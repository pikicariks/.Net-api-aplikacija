using System;
using API.Extensions;
using API.Intefaces;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Helpers;

public class LogUserActivity : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var result = await next();

        if (context.HttpContext.User.Identity?.IsAuthenticated!=true)
        {
            return;
        }

        var userid = result.HttpContext.User.GetUserId();

        var unitOfWork = result.HttpContext.RequestServices.GetRequiredService<IUnitOfWork>();
        var user = await unitOfWork.UserRepository.GetUserByIdAsync(userid);

        if(user==null) return;

        user.LastActive = DateTime.UtcNow;
        await unitOfWork.Complete();
    }
}
