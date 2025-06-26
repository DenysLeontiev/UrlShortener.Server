using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using UrlShortener.API.Data;
using UrlShortener.API.Interfaces.Authentication;
using UrlShortener.API.Interfaces.Persistence;
using UrlShortener.API.Interfaces.Urls;
using UrlShortener.API.Models.Entities;
using UrlShortener.API.Repositories;
using UrlShortener.API.Services.Authentication;
using UrlShortener.API.Services.Seed;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
builder.Services.AddControllers();

builder.Services.AddCors();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenHandler, UrlShortener.API.Services.Authentication.TokenHandler>();
builder.Services.AddScoped<ContextSeedService>();

builder.Services.AddScoped<IUrlRepository, UrlRepository>();

builder.Services.AddScoped<UrlShorteningService>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        return Task.CompletedTask;
    };
    options.Events.OnRedirectToAccessDenied = context =>
    {
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        return Task.CompletedTask;
    };
});


builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddHttpContextAccessor();

string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;

builder.Services.AddDbContext<UrlShortenerDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddAuthentication(options => {
    //options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(opts =>
{
    opts.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:TokenKey"]!)),
        ValidateIssuer = false,
        ValidateAudience = false,
    };
});


builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;    
    options.Password.RequireUppercase = false;
    options.User.AllowedUserNameCharacters = null!;
}).AddEntityFrameworkStores<UrlShortenerDbContext>().AddDefaultTokenProviders();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(opt =>
{
    opt.AllowAnyHeader().AllowAnyMethod().AllowCredentials().WithOrigins(builder.Configuration["JWT:ClientUrl"]!, builder.Configuration["JWT:Issuer"]!);
});

app.MapGet("{code}", async (string code, IUrlRepository urlRepository) =>
{
    var shortenedUrl = await urlRepository.GetOriginalUrlAsync(code);
    if (shortenedUrl == null)
    {
        return Results.NotFound();
    }
    return Results.Redirect(shortenedUrl.OriginalUrl);
});

app.UseHttpsRedirection();


app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using var scope = app.Services.CreateScope();

var contextSeedService = scope.ServiceProvider.GetRequiredService<ContextSeedService>();

await contextSeedService.ApplyPendingMigrationsAsync();

await contextSeedService.SeedUserRolesAsync();
await contextSeedService.SeedAdminUser();

app.Run();
