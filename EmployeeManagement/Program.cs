using EmployeeManagement.Models;
using EmployeeManagement.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using NLog.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;
var env = builder.Environment;
var logging = builder.Logging;

var services = builder.Services;

services.AddDbContextPool<AppDbContext>(options => options.UseSqlServer(config.GetConnectionString("EmployeeDBConnection")));

services.AddMvc(options =>
{
    var policy = new AuthorizationPolicyBuilder()
    .RequireAuthenticatedUser()
    .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
}).AddXmlSerializerFormatters();

services.AddAuthentication()
.AddGoogle(options =>
{
    options.ClientId = config["Authentication:Google:ClientId"];
    options.ClientSecret = config["Authentication:Google:ClientSecret"];
})
.AddFacebook(options =>
{
    options.AppId = config["Authentication:Facebook:AppId"];
    options.AppSecret = config["Authentication:Facebook:AppSecret"];
});


services.ConfigureApplicationCookie(options =>
{
    options.AccessDeniedPath = "/Administration/AccessDenied";
});

services.AddAuthorization(options =>
{
    options.AddPolicy("TestRolePolicy", policy => policy.RequireClaim("Delete Role").RequireClaim("Create Role"));

    options.AddPolicy("DeleteRolePolicy", policy => policy.RequireClaim("Delete Role", "true"));

    options.AddPolicy("EditRolePolicy", policy => policy.AddRequirements(new ManageAdminRolesAndClaimsRequirement()));

    options.AddPolicy("AdminRolePolicy", policy => policy.RequireRole("Admin"));
});

services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 8;
    options.Password.RequiredUniqueChars = 2;

    options.SignIn.RequireConfirmedEmail = true;

    options.Tokens.EmailConfirmationTokenProvider = "CustomEmailConfirmation";

    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders()
.AddTokenProvider<CustomEmailConfirmationTokenProvider<ApplicationUser>>("CustomEmailConfirmation");

// Changes token lifespan of all token types
services.Configure<DataProtectionTokenProviderOptions>(opt =>
                    opt.TokenLifespan = TimeSpan.FromHours(5));

// Changes token lifespan of just the email confirmation token type
services.Configure<CustomEmailConfirmationTokenProviderOptions>(opt =>
opt.TokenLifespan = TimeSpan.FromDays(3));

// Scoped Service: lifetime - one instance per request. Good for DB Contexts. Same instance in the scope of a given http request. New instance across different http requests. 
// When to use: Database contexts, repositories, unit of work pattern, per-request state

// Transient Service: lifetime - one instance per request. Bad for DB Contexts. New instance for every request. Good for lightweight, stateless services. 
// When to use: Lightweight services with no state, utility classes, simple operations

// Singleton Service: lifetime - one instance per application. Good for DB Contexts. Same instance for every request. 
// When to use: Configuration, caching, shared state, expensive-to-create objects

services.AddScoped<IEmployeeRepository, SQLEmployeeRepository>();

services.AddSingleton<IAuthorizationHandler, CanEditOnlyOtherAdminRolesAndClaimsHandler>();
services.AddSingleton<IAuthorizationHandler, SuperAdminHandler>();
services.AddSingleton<DataProtectionPurposeStrings>();

logging.AddConfiguration(config.GetSection("Logging"));
logging.AddConsole();
logging.AddDebug();
logging.AddEventSourceLogger();
logging.AddNLog();

var app = builder.Build();

// DefaultFilesOptions defaultFilesOptions = new();
// defaultFilesOptions.DefaultFileNames.Clear();
// defaultFilesOptions.DefaultFileNames.Add("foo.html");

// app.UseDefaultFiles(defaultFilesOptions);

// FileServerOptions fileServerOptions = new();
// fileServerOptions.DefaultFilesOptions.DefaultFileNames.Clear();
// fileServerOptions.DefaultFilesOptions.DefaultFileNames.Add("foo.html");

// app.UseFileServer(fileServerOptions); // Enables serving default files and static files

if (env.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseStatusCodePagesWithReExecute("/Error/{0}");
}

app.UseStaticFiles();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.MapGet("/env", () => $"{env.EnvironmentName} environment");
app.MapGet("/process", () => System.Diagnostics.Process.GetCurrentProcess().ProcessName);
app.MapGet("/key", () => config["MyKey"]);
// app.MapGet("/", () => "Hello World!");

app.Run();
