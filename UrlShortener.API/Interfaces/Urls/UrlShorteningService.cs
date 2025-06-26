using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using UrlShortener.API.Data;

namespace UrlShortener.API.Interfaces.Urls;

public class UrlShorteningService
{
    public const int NUMBER_OF_SYMBOLS_IN_SHORT_LINK = 7;
    private const string ALPHABET = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

    private readonly Random _random = new Random();
    private readonly UrlShortenerDbContext _dbContext;

    public UrlShorteningService(UrlShortenerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<string> GenerateUniqueCodeAsync()
    {
        var codeChars = new char[NUMBER_OF_SYMBOLS_IN_SHORT_LINK];

        while (true)
        {
            for (int i = 0; i < NUMBER_OF_SYMBOLS_IN_SHORT_LINK; i++)
            {
                var randomIndex = _random.Next(ALPHABET.Length - 1);

                codeChars[i] = ALPHABET[randomIndex];
            }

            var code = new string(codeChars);

            if(!await _dbContext.Urls.AnyAsync(x => x.Code.Equals(code)))
            {
                return code;
            }
        }
    }
}
