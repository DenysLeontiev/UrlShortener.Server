using System.Text.Json;
using UrlShortener.API.Pagination;

namespace UrlShortener.API.ExtensionMethods;

public static class HttpExtensions
{
    public static void AddPaginationHeaders(this HttpResponse response, PaginationHeader headers)
    {
        // be default is JsonNamingPolicy.PascalCase, but client understands CamelCase
        var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        response.Headers.Add("Pagination", JsonSerializer.Serialize(headers, jsonOptions)); // send as a header
        response.Headers.Add("Access-Control-Expose-Headers", "Pagination"); // to avoid CORS with headers
    }
}
