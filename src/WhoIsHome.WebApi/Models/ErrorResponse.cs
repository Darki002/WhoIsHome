namespace WhoIsHome.WebApi.Models;

public record ErrorResponse
{
    public required IEnumerable<string> Errors { get; set; }
}