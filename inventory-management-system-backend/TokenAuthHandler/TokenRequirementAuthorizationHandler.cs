using Microsoft.AspNetCore.Authorization;

namespace inventory_management_system_backend.TokenAuthHandler
{
    public class TokenRequirementAuthorizationHandler : AuthorizationHandler<TokenRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, TokenRequirement requirement)
        {
            Console.WriteLine("This is being executed!");
            if (!context.User.HasClaim(x => x.Type == "type"))
            {
                context.Fail();
                return Task.CompletedTask;
            }

            var type = context.User.Claims.FirstOrDefault(x => x.Type == "type").Value;

            if (type == "bearer")
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }

            return Task.CompletedTask;
        }
    }
}