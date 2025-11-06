namespace Api.Dtos.Auth
{
    public record RegisterDto(string Username, string Password, string FirstName, string LastName, string? Email, string Role = "EMPLOYEE");
}
