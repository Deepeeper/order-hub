using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace OrderHub.Api.Authentication;

/// <summary>
/// Development-only authentication scheme. Every request is auto-authenticated
/// as a synthetic user. In production this is replaced by JWT bearer authentication
/// against Microsoft Entra ID, per the Ovako migration defaults
/// (golden-path/steps/Step-05-Supplements.md).
///
/// Authorization is enforced via <c>RequireAuthorization()</c> on endpoints —
/// swapping the scheme is then a one-line change in Program.cs.
/// </summary>
public sealed class DevBypassHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string SchemeName = "DevBypass";

    public DevBypassHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "dev-user"),
            new Claim(ClaimTypes.Name, "Dev User"),
        };
        var identity = new ClaimsIdentity(claims, SchemeName);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, SchemeName);
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
