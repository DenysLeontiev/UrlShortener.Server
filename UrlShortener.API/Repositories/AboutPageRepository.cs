using Microsoft.EntityFrameworkCore;
using UrlShortener.API.Data;
using UrlShortener.API.Interfaces.Persistence;
using UrlShortener.API.Models.Entities;

namespace UrlShortener.API.Repositories;

public class AboutPageRepository : IAboutPageRepository
{
    private readonly UrlShortenerDbContext _dbContext;

    public AboutPageRepository(UrlShortenerDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<AboutPage> Get()
    {
        var aboutPage = await _dbContext.AboutPages!
            .AsNoTracking()
            .OrderByDescending(x => x.DateCreated)
            .Include(x => x.EditedBy)
            .FirstOrDefaultAsync();

        return aboutPage!;
    }

    public async Task<AboutPage> Edit(string newContent, string userId)
    {
        var aboutPageToEdit = await _dbContext.AboutPages
            .OrderByDescending(x => x.DateCreated)
            .FirstOrDefaultAsync();

        aboutPageToEdit!.EditedById = userId;
        aboutPageToEdit.Content = newContent;

        await _dbContext.SaveChangesAsync();

        return aboutPageToEdit;
    }
}
