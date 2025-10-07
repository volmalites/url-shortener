// Data Transfer Objects

public class RegisterDto
{
    public required string Username { get; set; }
    public required string Password { get; set; }
}

public class CreateUrlDto
{
    public required string OriginalUrl { get; set; }
}
