using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace BoxSample.Security
{
    public class BoxAuthenticationHandler : AuthenticationHandler<BoxAuthenticationSchemeOptions>
    {
        public BoxAuthenticationHandler(IOptionsMonitor<BoxAuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            ITicketService ticketService)
            : base(options, logger, encoder, clock)
        {
            TicketService = ticketService;
        }

        public ITicketService TicketService { get; }

        protected async override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var authorization = Request.Headers["Authorization"];
            if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
            {
                var authToken = headerValue.Parameter;
                if (string.IsNullOrEmpty(authToken))
                    return AuthenticateResult.Fail("Invalid token.");

                var ticket = await TicketService.ValidateToken(authToken);
                if (ticket != null)
                    return AuthenticateResult.Success(ticket);
                return AuthenticateResult.Fail("Invalid user.");
            }
            return AuthenticateResult.Fail("Incorrect token format.");
        }
    }
}
