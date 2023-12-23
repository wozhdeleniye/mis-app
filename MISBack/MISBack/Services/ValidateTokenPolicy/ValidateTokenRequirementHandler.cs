using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using MISBack.Data;
using System.Text.RegularExpressions;

namespace MISBack.Services.ValidateTokenPolicy
{
    public class ValidateTokenRequirementHandler : AuthorizationHandler<ValidateTokenRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public ValidateTokenRequirementHandler(IHttpContextAccessor httpContextAccessor,
            IServiceScopeFactory serviceScopeFactory)
        {
            _httpContextAccessor = httpContextAccessor;
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
            ValidateTokenRequirement requirement)
        {
            if (_httpContextAccessor.HttpContext != null)
            {
                string? authorizationString = _httpContextAccessor.HttpContext.Request.Headers[HeaderNames.Authorization];
                if (authorizationString == null)
                {
                    var ex = new Exception();
                    ex.Data.Add(StatusCodes.Status401Unauthorized.ToString(),
                        "Access token not found"
                    );
                    throw ex;
                }

                var token = GetToken(authorizationString);

                using var scope = _serviceScopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var tokenEntity = await dbContext
                    .Tokens
                    .Where(x => x.InvalidToken == token)
                    .FirstOrDefaultAsync();

                if (tokenEntity != null)
                {
                    var ex = new Exception();
                    ex.Data.Add(StatusCodes.Status401Unauthorized.ToString(),
                        "Not authorized"
                    );
                    throw ex;
                }

                context.Succeed(requirement);
            }
            else
            {
                var ex = new Exception();
                ex.Data.Add(StatusCodes.Status400BadRequest.ToString(),
                    "Bad request"
                );
                throw ex;
            }
        }

        private static string GetToken(string authorizationString)
        {
            const string pattern = @"\S+\.\S+\.\S+";
            var regex = new Regex(pattern);
            var matches = regex.Matches(authorizationString);

            if (matches.Count <= 0)
            {
                var ex = new Exception();
                ex.Data.Add(StatusCodes.Status401Unauthorized.ToString(),
                    "Not authorized"
                );
                throw ex;
            }

            var token = matches[0].Value;

            return token;
        }
    }
}
