using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace LoopbackAuthenticationIntegrationPoC.Authentication
{
    public static class OpaqueTokenExtensions
    {
        public static AuthenticationBuilder AddOpaqueToken(this AuthenticationBuilder builder)
        {
            return builder.AddScheme<OpaqueTokenOptions, OpaqueTokenHandler>(OpaqueTokenDefaults.AuthenticationScheme, c => {});
        }
    }
}
