using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoopbackAuthenticationIntegrationPoC.Authentication;
using LoopbackAuthenticationIntegrationPoC.Data;
using LoopbackAuthenticationIntegrationPoC.Entities;
using LoopbackAuthenticationIntegrationPoC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace LoopbackAuthenticationIntegrationPoC.Controllers
{
    [Route("api/account")]
    public class AccountController : ControllerBase
    {
        private readonly LoopbackDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AccountController(LoopbackDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        [Route("login")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<AccessToken>> Login([FromBody] LoginCredentials credentials)
        {
            var user = await _context.Users.Include(u => u.AccessTokens).FirstOrDefaultAsync(u => u.Username == credentials.Username);
            if (user == null)
            {
                return BadRequest(new ErrorDetails
                {
                    Message = "Invalid username or password"
                });
            }

            var isPasswordOk = BCrypt.Net.BCrypt.Verify(credentials.Password, user.PasswordHash);
            if (!isPasswordOk)
            {
                return BadRequest(new ErrorDetails
                {
                    Message = "Invalid username or password"
                });
            }

            // Note: this token is not compatible with loopback. Loopback encodes information 
            // into the token, we'll just generate a random guid to use as a token
            var accessToken = new AccessToken();
            user.AccessTokens.Add(accessToken);
            await _context.SaveChangesAsync();

            return Ok(accessToken);
        }

        [Route("logout")]
        [HttpPost]
        // Explicitly configured authentication scheme, this could be set to all
        // routes instead of just this at startup.
        [Authorize(AuthenticationSchemes = OpaqueTokenDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Logout()
        {
            var tokenClaim = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == CustomClaimTypes.TokenId);

            if (tokenClaim == null)
            {
                return Unauthorized();
            }

            var token = await _context.AccessTokens.FindAsync(tokenClaim.Value);

            if (token == null)
            {
                return Unauthorized();
            }

            _context.AccessTokens.Remove(token);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [Route("")]
        [HttpPut]
        [Authorize(AuthenticationSchemes = OpaqueTokenDefaults.AuthenticationScheme)]
        public async Task<ActionResult> UpdateUser([FromBody] UserDetails details)
        {
            var tokenClaim = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == CustomClaimTypes.TokenId);

            if (tokenClaim == null)
            {
                return Unauthorized();
            }

            var token = await _context.AccessTokens.Include(at => at.User).FirstOrDefaultAsync(at => at.Id == tokenClaim.Value);
            if (token == null)
            {
                return Unauthorized();
            }

            token.User.PasswordHash = BCrypt.Net.BCrypt.HashPassword(details.Password);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
