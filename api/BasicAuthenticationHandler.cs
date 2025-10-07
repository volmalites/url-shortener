using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Data.Sqlite;
using Dapper;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using BCrypt.Net;

public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public BasicAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.ContainsKey("Authorization"))
            return Task.FromResult(AuthenticateResult.Fail("Missing Authorization Header"));

        try
        {
            var authHeader = Request.Headers["Authorization"].ToString();
            if (authHeader.StartsWith("Basic "))
            {
                var encoded = authHeader.Substring(6).Trim();
                var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(encoded));
                var parts = decoded.Split(':', 2);
                if (parts.Length == 2)
                {
                    var username = parts[0];
                    var password = parts[1];

                    using var conn = new SqliteConnection("Data Source=app.db");
                    var user = conn.QueryFirstOrDefault<UserRecord>("SELECT * FROM Users WHERE Username = @Username", new { Username = username });
                    if (user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                    {
                        var claims = new[] { new Claim(ClaimTypes.Name, username) };
                        var identity = new ClaimsIdentity(claims, Scheme.Name);
                        var principal = new ClaimsPrincipal(identity);
                        var ticket = new AuthenticationTicket(principal, Scheme.Name);
                        return Task.FromResult(AuthenticateResult.Success(ticket));
                    }
                }
            }
        }
        catch
        {
            // Do nothing here
        }

        return Task.FromResult(AuthenticateResult.Fail("Invalid Authorization Header"));
    }
}

public class UserRecord
{
    public int Id { get; set; }
    public string Username { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public double WalletBalance { get; set; }
    public int TotalClicks { get; set; }
}

public class ShortUrlRecord
{
    public int Id { get; set; }
    public string ShortCode { get; set; } = "";
    public string OriginalUrl { get; set; } = "";
    public int UserId { get; set; }
    public int Clicks { get; set; }
}
