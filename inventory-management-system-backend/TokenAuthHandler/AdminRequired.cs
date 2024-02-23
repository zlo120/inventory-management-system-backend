using Microsoft.AspNetCore.Authorization;

namespace inventory_management_system_backend.TokenAuthHandler
{
    public class AdminRequired : IAuthorizationRequirement
    {
        public AdminRequired()
        {

        }
    }
}
