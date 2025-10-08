using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Dapper;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:8080")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddAuthentication("BasicAuthentication")
    .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();

// Database initialization
var connectionString = "Data Source=app.db";
using (var conn = new SqliteConnection(connectionString))
{
    conn.Open();
    conn.Execute(@"
        CREATE TABLE IF NOT EXISTS Users (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Username TEXT UNIQUE NOT NULL,
            PasswordHash TEXT NOT NULL,
            WalletBalance REAL DEFAULT 0.0,
            TotalClicks INTEGER DEFAULT 0
        )");
    conn.Execute(@"
        CREATE TABLE IF NOT EXISTS ShortUrls (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            ShortCode TEXT UNIQUE NOT NULL,
            OriginalUrl TEXT NOT NULL,
            UserId INTEGER NOT NULL,
            Clicks INTEGER DEFAULT 0,
            FOREIGN KEY(UserId) REFERENCES Users(Id)
        )");
}

string GenerateUniqueShortCode()
{
    var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
    var random = new Random();
    string code;
    do
    {
        code = new string(Enumerable.Repeat(chars, 5).Select(s => s[random.Next(s.Length)]).ToArray());
    } while (ShortCodeExists(code));
    return code;
}

bool ShortCodeExists(string code)
{
    using var conn = new SqliteConnection(connectionString);
    return conn.QueryFirstOrDefault<int>("SELECT COUNT(1) FROM ShortUrls WHERE ShortCode = @ShortCode", new { ShortCode = code }) > 0;
}

UserRecord? GetUser(string username)
{
    using var conn = new SqliteConnection(connectionString);
    return conn.QueryFirstOrDefault<UserRecord>("SELECT * FROM Users WHERE Username = @Username", new { Username = username });
}

int GetUserId(string username)
{
    using var conn = new SqliteConnection(connectionString);
    return conn.QueryFirstOrDefault<int>("SELECT Id FROM Users WHERE Username = @Username", new { Username = username });
}

ShortUrlRecord? GetShortUrl(string shortCode)
{
    using var conn = new SqliteConnection(connectionString);
    return conn.QueryFirstOrDefault<ShortUrlRecord>("SELECT * FROM ShortUrls WHERE ShortCode = @ShortCode", new { ShortCode = shortCode });
}

void IncrementClick(int urlId)
{
    using var conn = new SqliteConnection(connectionString);
    conn.Execute("UPDATE ShortUrls SET Clicks = Clicks + 1 WHERE Id = @Id", new { Id = urlId });
}

UserRecord? GetUserById(int userId)
{
    using var conn = new SqliteConnection(connectionString);
    return conn.QueryFirstOrDefault<UserRecord>("SELECT * FROM Users WHERE Id = @Id", new { Id = userId });
}

void UpdateUser(UserRecord user)
{
    using var conn = new SqliteConnection(connectionString);
    conn.Execute("UPDATE Users SET WalletBalance = @WalletBalance, TotalClicks = @TotalClicks WHERE Id = @Id",
        new { user.WalletBalance, user.TotalClicks, user.Id });
}

double CalculatePercentage(int clicks)
{
    return Math.Min(10 + 10 * (clicks / 5), 80);
}

int GetTotalClicksForUser(int userId)
{
    using var conn = new SqliteConnection(connectionString);
    return conn.QueryFirstOrDefault<int?>("SELECT SUM(Clicks) FROM ShortUrls WHERE UserId = @UserId", new { UserId = userId }) ?? 0;
}

app.MapGet("/{shortCode}", [AllowAnonymous] (string shortCode) =>
{
    var url = GetShortUrl(shortCode);
    if (url == null) return Results.NotFound();

    IncrementClick(url.Id);

    var user = GetUserById(url.UserId);
    if (user != null)
    {
        user.TotalClicks = GetTotalClicksForUser(user.Id);
        var perc = CalculatePercentage(user.TotalClicks);
        var earn = (perc / 100.0) * 10.0;
        user.WalletBalance += earn;
        UpdateUser(user);
    }

    return Results.Redirect(url.OriginalUrl);
});

app.MapPost("/api/auth/register", [AllowAnonymous] ([FromBody] RegisterDto dto) =>
{
    if (string.IsNullOrEmpty(dto.Username) || string.IsNullOrEmpty(dto.Password))
        return Results.BadRequest("Invalid username or password");

    using var conn = new SqliteConnection(connectionString);
    if (conn.QueryFirstOrDefault<int>("SELECT COUNT(1) FROM Users WHERE Username = @Username", new { Username = dto.Username }) > 0)
        return Results.BadRequest("Username already exists");

    var hash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
    conn.Execute("INSERT INTO Users (Username, PasswordHash, WalletBalance, TotalClicks) VALUES (@Username, @PasswordHash, 0.0, 0)",
        new { Username = dto.Username, PasswordHash = hash });

    return Results.Ok("User registered");
});

app.MapPost("/api/urls", [Authorize] ([FromBody] CreateUrlDto dto, HttpContext ctx) =>
{
    var username = ctx.User.Identity?.Name;
    if (username == null) return Results.Unauthorized();

    var user = GetUser(username);
    if (user == null) return Results.Unauthorized();

    if (string.IsNullOrEmpty(dto.OriginalUrl)) return Results.BadRequest("Invalid URL");

    var shortCode = GenerateUniqueShortCode();
    using var conn = new SqliteConnection(connectionString);
    conn.Execute("INSERT INTO ShortUrls (ShortCode, OriginalUrl, UserId, Clicks) VALUES (@ShortCode, @OriginalUrl, @UserId, 0)",
        new { ShortCode = shortCode, OriginalUrl = dto.OriginalUrl, UserId = user.Id });

    return Results.Ok(new { ShortUrl = $"http://localhost:1337/{shortCode}" });
});

app.MapGet("/api/urls", [Authorize] (HttpContext ctx) =>
{
    var username = ctx.User.Identity?.Name;
    if (username == null) return Results.Unauthorized();

    var userId = GetUserId(username);
    using var conn = new SqliteConnection(connectionString);
    var urls = conn.Query<ShortUrlRecord>("SELECT ShortCode, OriginalUrl, Clicks FROM ShortUrls WHERE UserId = @UserId", new { UserId = userId });
    return Results.Ok(urls);
});

app.MapGet("/api/wallet", [Authorize] (HttpContext ctx) =>
{
    var username = ctx.User.Identity?.Name;
    if (username == null) return Results.Unauthorized();

    var user = GetUser(username);
    if (user == null) return Results.Unauthorized();

    var totalClicks = GetTotalClicksForUser(user.Id);
    var percentage = CalculatePercentage(totalClicks);
    var totalBalance = totalClicks * 10.0;
    var balance = (percentage / 100.0) * totalBalance; // Recalculate based on current percentage
    
    var growthInfo = $"Current share: {percentage}%.";
    if (percentage < 80.0)
    {
        var nextIncreaseClicks = (totalClicks / 5 + 1) * 5;
        growthInfo += $" Next increase at {nextIncreaseClicks} total clicks.";
    }

    return Results.Ok(new { Balance = balance, TotalBalance = totalBalance, Percentage = percentage, GrowthInfo = growthInfo, TotalClicks = totalClicks });
});

app.MapPost("/api/urls/click/{shortCode}", [AllowAnonymous] (string shortCode) =>
{
    var url = GetShortUrl(shortCode);
    if (url == null) return Results.NotFound();

    IncrementClick(url.Id);

    var user = GetUserById(url.UserId);
    if (user != null)
    {
        user.TotalClicks = GetTotalClicksForUser(user.Id);
        var perc = CalculatePercentage(user.TotalClicks);
        var earn = (perc / 100.0) * 10.0;
        user.WalletBalance += earn;
        UpdateUser(user);
    }

    return Results.Ok(new { Message = $"Click recorded for {shortCode}" });
});

app.Run();
