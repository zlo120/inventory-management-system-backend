using Core.Models;
using Microsoft.AspNetCore.Authorization;

namespace inventory_management_system_backend.TokenAuthHandler
{
    public class AdminRequiredAuthorizationHandler : AuthorizationHandler<AdminRequired>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AdminRequiredAuthorizationHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AdminRequired requirement)
        {

            if (!context.User.HasClaim(x => x.Type == "type"))
            {
                context.Fail();
                return Task.CompletedTask;
            }

            var type = context.User.Claims.FirstOrDefault(x => x.Type == "type").Value;

            if (type != "bearer")
            {
                SetCustomResponse("Invalid token type. Only 'bearer' tokens are allowed.");
                context.Fail();
                return Task.CompletedTask;
            }

            var group = context.User.Claims.FirstOrDefault(x => x.Type == "group_id").Value;

            if (!(int.Parse(group) >= (int)UserGroups.Admin))
            {
                SetCustomResponse("Only admins are allowed to access this endpoint.");
                context.Fail();
                return Task.CompletedTask;
            }

            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        private void SetCustomResponse(string message)
        {
            var response = _httpContextAccessor.HttpContext.Response;
            response.StatusCode = StatusCodes.Status401Unauthorized;
            response.ContentType = "application/json";

            var responseData = new
            {
                Message = message,
                // Add any additional data you want to include in the response
            };

            response.WriteAsync(Newtonsoft.Json.JsonConvert.SerializeObject(responseData));
        }
    }
}
