using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using WhoIsHome.Entities;

namespace WhoIsHome.AuthTokens;

[Table("RefreshToken")]
[Index(nameof(Token), IsUnique = true)]
public class RefreshToken()
{
    private const int ExpiresInDays = 14;
    private const int RefreshTokenLength = 64;
    
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Token { get; set; } = null!;

    [Required]
    public DateTime Issued { get; set; }

    public DateTime ExpiredAt { get; set; }
    
    [Required]
    public int UserId { get; set; }
    
    [Required]
    public User User { get; set; } = null!;
    
    public RefreshToken(int userId, string token, DateTime issued, DateTime expiredAt) : this()
    {
        Token = token;
        Issued = issued;
        ExpiredAt = expiredAt;
        UserId = userId;
    }

    public static RefreshToken Generate(int userId, DateTime currentTime)
    {
        var token = GenerateToken();
        var expiresAt = currentTime.AddDays(ExpiresInDays);
        return new RefreshToken(userId, token, currentTime, expiresAt);
    }
    
    private static string GenerateToken()
    {
        var randomNumber = new byte[RefreshTokenLength];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}