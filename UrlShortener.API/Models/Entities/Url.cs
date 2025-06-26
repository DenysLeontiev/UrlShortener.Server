using System.ComponentModel.DataAnnotations.Schema;

namespace UrlShortener.API.Models.Entities;

public class Url : BaseEntity
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string Id { get; set; } = string.Empty;

    public string OriginalUrl { get; set; } = string.Empty;
    public string ShortenUrl { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;

    public User User { get; set; }
    public string UserId { get; set; } = string.Empty;
}
