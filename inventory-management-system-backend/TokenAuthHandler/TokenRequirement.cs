using Microsoft.AspNetCore.Authorization;

namespace inventory_management_system_backend.TokenAuthHandler
{
    public class TokenRequirement : IAuthorizationRequirement
    {
        public TokenRequirement()
        {
            
        }
    }
}
