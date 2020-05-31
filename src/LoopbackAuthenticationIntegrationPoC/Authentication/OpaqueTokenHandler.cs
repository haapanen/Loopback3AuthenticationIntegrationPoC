using System;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using LoopbackAuthenticationIntegrationPoC.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LoopbackAuthenticationIntegrationPoC.Authentication
{
    public class OpaqueTokenHandler : AuthenticationHandler<OpaqueTokenOptions>
    {
        private const string AuthorizationHeaderName = "Authorization";
     
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly LoopbackDbContext _context;

        public OpaqueTokenHandler(IOptionsMonitor<OpaqueTokenOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, IHttpContextAccessor httpContextAccessor, LoopbackDbContext context) : base(options, logger, encoder, clock)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!_httpContextAccessor.HttpContext.Request.Headers.ContainsKey(AuthorizationHeaderName))
            {
                return AuthenticateResult.NoResult();
            }

            if(!AuthenticationHeaderValue.TryParse(Request.Headers[AuthorizationHeaderName], out AuthenticationHeaderValue headerValue))
            {
                return AuthenticateResult.NoResult();
            }

            if (headerValue.Scheme != OpaqueTokenDefaults.AuthenticationScheme)
            {
                return AuthenticateResult.NoResult();
            }

            var tokenText = headerValue.Parameter;
            var token = await _context.AccessTokens.Include(at => at.User).FirstOrDefaultAsync(at => at.Id == tokenText);
            if (token == null || token.Created.AddSeconds(token.Ttl) < DateTime.UtcNow)
            {
                return AuthenticateResult.NoResult();
            }

            var claims = new []
            {
                new Claim(ClaimTypes.Name, token.User.Username),
                new Claim(CustomClaimTypes.TokenId, token.Id), 
                new Claim(ClaimTypes.Role, "ExampleRole"), 
            };

            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }
    }
}