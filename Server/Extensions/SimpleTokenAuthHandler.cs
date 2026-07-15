using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Server.Extensions
{
    public class SimpleTokenAuthOptions : AuthenticationSchemeOptions { }

    public class SimpleTokenAuthHandler : AuthenticationHandler<SimpleTokenAuthOptions>
    {
        private readonly IConfiguration _configuration;

        public SimpleTokenAuthHandler(
            IOptionsMonitor<SimpleTokenAuthOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            IConfiguration configuration) : base(options, logger, encoder)
        {
            _configuration = configuration;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue("X-Admin-Token", out var tokenValues))
            {
                return Task.FromResult(AuthenticateResult.NoResult());
            }

            var token = tokenValues.ToString();
            var expectedToken = _configuration["JWT_SECRET"] ?? "super_secret_development_token_key_123456";

            if (token == expectedToken)
            {
                var claims = new[] { new Claim(ClaimTypes.Name, "admin") };
                var identity = new ClaimsIdentity(claims, Scheme.Name);
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, Scheme.Name);
                return Task.FromResult(AuthenticateResult.Success(ticket));
            }

            return Task.FromResult(AuthenticateResult.Fail("Invalid admin token"));
        }
    }
}
