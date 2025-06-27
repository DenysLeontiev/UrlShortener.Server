using System.ComponentModel.DataAnnotations.Schema;

namespace UrlShortener.API.Models.Entities;

public class AboutPage : BaseEntity
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string Id { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public string EditedById { get; set; } = string.Empty;
    public User EditedBy { get; set; } = new();
}
