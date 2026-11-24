using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Polly;
using Polly.Extensions.Http;
using RealEstateAgency.Application.Agents;
using RealEstateAgency.Application.Common;
using RealEstateAgency.Application.Deals;
using RealEstateAgency.Application.Dictionaries;
using RealEstateAgency.Application.Integration;
using RealEstateAgency.Application.Properties;
using RealEstateAgency.Infrastructure;
using RealEstateAgency.Infrastructure.Identity;
using RealEstateAgency.Infrastructure.Integration;
using RealEstateAgency.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Password.RequiredLength = 6;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireDigit = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
        options.User.RequireUniqueEmail = true;
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("OnlyAdults", policy =>
        policy.RequireAuthenticatedUser()
            .RequireAssertion(context =>
            {
                var claim = context.User.FindFirst("RegisteredAt");
                return claim is not null
                    && DateTime.TryParse(claim.Value, out var registeredAt)
                    && registeredAt <= DateTime.Now.AddYears(-1);
            }));
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.SlidingExpiration = true;
});

builder.Services.AddScoped<IPropertyService, PropertyService>();
builder.Services.AddScoped<IDealService, DealService>();
builder.Services.AddScoped<IAgentService, AgentService>();
builder.Services.AddScoped<IDictionaryService, DictionaryService>();

builder.Services.Configure<RealEstateOptions>(
    builder.Configuration.GetSection(RealEstateOptions.SectionName));

builder.Services.AddControllersWithViews();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "RealEstateAgency API",
        Version = "v1",
        Description = "Учебный API агентства недвижимости — МДК 05.02, вариант 3"
    });
});

builder.Services.AddHttpClient<IMortgageApiClient, MortgageApiClient>((client, services) =>
    {
        var options = services.GetRequiredService<IOptions<RealEstateOptions>>().Value;

        client.BaseAddress = new Uri(options.MortgageApiBaseUrl);
        client.Timeout = TimeSpan.FromSeconds(Math.Max(1, options.MortgageApiTimeoutSeconds));
        client.DefaultRequestHeaders.Add("Accept", "application/json");
    })
    .AddPolicyHandler(GetRetryPolicy())
    .AddPolicyHandler(GetCircuitBreakerPolicy());

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
    if (pendingMigrations.Any())
    {
        await context.Database.MigrateAsync();
    }
    else
    {
        await context.Database.EnsureCreatedAsync();
    }

    await DbSeeder.SeedAsync(scope.ServiceProvider);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "RealEstateAgency API v1");
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy() =>
    HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrResult(message => message.StatusCode == HttpStatusCode.TooManyRequests)
        .WaitAndRetryAsync(3, attempt => TimeSpan.FromMilliseconds(200 * Math.Pow(2, attempt)));

static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy() =>
    HttpPolicyExtensions
        .HandleTransientHttpError()
        .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));
