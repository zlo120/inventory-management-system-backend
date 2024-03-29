﻿using Microsoft.AspNetCore.Authorization;

namespace inventory_management_system_backend.TokenAuthHandler
{
    public class TokenRequirementAuthorizationHandler : AuthorizationHandler<TokenRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public TokenRequirementAuthorizationHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, TokenRequirement requirement)
        {
            if (!context.User.HasClaim(x => x.Type == "type"))
            {
                context.Fail();
                return Task.CompletedTask;
            }

            var type = context.User.Claims.FirstOrDefault(x => x.Type == "type").Value;

            var endpoint = _httpContextAccessor.HttpContext.GetEndpoint();

            if (IsRefreshEndpoint(endpoint) && type == "refresh")
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            if (type == "bearer")
            {
                context.Succeed(requirement);
            }
            else
            {
                SetCustomResponse("Invalid token type. Only 'bearer' tokens are allowed.");
                context.Fail();
            }

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

        private bool IsRefreshEndpoint(Endpoint endpoint)
        {
            if (endpoint != null)
            {
                var path = _httpContextAccessor.HttpContext.Request.Path;
                var excludedPath = "/api/User/Refresh";

                return path.Equals(excludedPath, StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }
    }
}