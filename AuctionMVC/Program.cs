using AuctionMVC.Filters;
using AuctionMVC.Options;
using AuctionMVC.Services;
using AuctionMVC.Services.Api;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// -----------------------------------------------------------------------------
// MVC + Filters
// -----------------------------------------------------------------------------
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<HandleApiErrorFilter>();
});

builder.Services.AddHttpContextAccessor();

// -----------------------------------------------------------------------------
// Strongly typed configuration
// -----------------------------------------------------------------------------
builder.Services.Configure<ApiOptions>(builder.Configuration.GetSection(ApiOptions.SectionName));
builder.Services.Configure<AuthOptions>(builder.Configuration.GetSection(AuthOptions.SectionName));

// -----------------------------------------------------------------------------
// Authentication — Cookie-based; JWT is forwarded to the API as a Bearer token.
// -----------------------------------------------------------------------------
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.Cookie.Name = "AuctionMVC.Auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();

// -----------------------------------------------------------------------------
// Typed HTTP Clients (IHttpClientFactory) — the ONLY gateway to AuctionAPI.
// Each client is registered via its interface so dependent services resolve
// the typed client through the DI container correctly.
// -----------------------------------------------------------------------------
builder.Services.AddHttpClient<IAuctionsApiClient, AuctionsApiClient>();
builder.Services.AddHttpClient<IProductsApiClient, ProductsApiClient>();
builder.Services.AddHttpClient<IUsersApiClient, UsersApiClient>();
builder.Services.AddHttpClient<IBidsApiClient, BidsApiClient>();
builder.Services.AddHttpClient<IWinnersApiClient, WinnersApiClient>();
builder.Services.AddHttpClient<IAuthApiClient, AuthApiClient>();

// -----------------------------------------------------------------------------
// Application services (Clean Architecture presentation orchestration)
// -----------------------------------------------------------------------------
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IAuctionManagementService, AuctionManagementService>();
builder.Services.AddScoped<IProductManagementService, ProductManagementService>();
builder.Services.AddScoped<IUserManagementService, UserManagementService>();
builder.Services.AddScoped<IBidManagementService, BidManagementService>();
builder.Services.AddScoped<IWinnerManagementService, WinnerManagementService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();

var app = builder.Build();

// -----------------------------------------------------------------------------
// HTTP pipeline
// -----------------------------------------------------------------------------
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

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();

