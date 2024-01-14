using Microsoft.AspNetCore.Authorization;

namespace MISBack.Services.ValidateTokenPolicy
{
    public class ValidateTokenRequirement : IAuthorizationRequirement
    {
        public ValidateTokenRequirement()
        {
        }
    }
}
