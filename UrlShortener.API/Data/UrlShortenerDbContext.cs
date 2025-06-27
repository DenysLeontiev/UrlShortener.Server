using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UrlShortener.API.Models.Entities;

namespace UrlShortener.API.Data;

public class UrlShortenerDbContext : IdentityDbContext<User>
{
    public UrlShortenerDbContext(DbContextOptions<UrlShortenerDbContext> options) : base(options)   
    {
        
    }
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        Save();
        return await base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        Save();
        return base.SaveChanges();
    }

    private void Save()
    {
        foreach (var entry in base.ChangeTracker.Entries<BaseEntity>()
                     .Where(q => q.State == EntityState.Added || q.State == EntityState.Modified))
        {
            entry.Entity.DateModified = DateTime.UtcNow;

            if (entry.State == EntityState.Added)
            {
                entry.Entity.DateCreated = DateTime.UtcNow;
            }
        }
    }

    public DbSet<Url> Urls { get; set; }
    public DbSet<AboutPage> AboutPages { get; set; }
}

