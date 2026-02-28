using System.Net.Mime;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi;
using MySecureBackend.WebApi.Repositories;
using MySecureBackend.WebApi.Repositories.Environment2D;
using MySecureBackend.WebApi.Repositories.Object2D;
using MySecureBackend.WebApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Register MVC controllers for handling HTTP requests.
builder.Services.AddControllers();

// Retrieve the SQL connection string from configuration.
var sqlConnectionString = builder.Configuration.GetValue<string>("SqlConnectionString");
var sqlConnectionStringFound = !string.IsNullOrWhiteSpace(sqlConnectionString);

// Register OpenAPI/Swagger for API documentation and testing.
//builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Corvo's backend",
        Version = "v1"
    });
});

builder.Services.Configure<RouteOptions>(o => o.LowercaseUrls = true);

// Register authorization services for securing endpoints.
builder.Services.AddAuthorization();

// Register ASP.NET Core Identity with Dapper stores for user authentication and management.
// Configures password and user requirements.
builder.Services.AddIdentityApiEndpoints<IdentityUser>(options =>
    {
        options.User.RequireUniqueEmail = true;
        options.Password.RequiredLength = 8;
    })
    .AddRoles<IdentityRole>()
    .AddDapperStores(options => { options.ConnectionString = sqlConnectionString; });

// Register IHttpContextAccessor for accessing HTTP context in services (e.g., to get current user info).
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<IAuthenticationService, AspNetIdentityAuthenticationService>();

// Register application repositories.
// By default, use an in-memory repository for example objects.
builder.Services.AddTransient<IExampleObjectRepository, MemoryExampleObjectRepository>();
//builder.Services.AddTransient<IEnvironmentRepository, MemoryEnvironment2DRepository>();
//builder.Services.AddTransient<IObjectRepository, MemoryObject2DRepository>();

// To use a SQL-backed repository instead, uncomment the following line:
//builder.Services.AddTransient<IExampleObjectRepository, SqlExampleObjectRepository>(o => new SqlExampleObjectRepository(sqlConnectionString!));
builder.Services.AddTransient<IEnvironmentRepository, SqlEnvironment2dRepository>(o =>
    new SqlEnvironment2dRepository(sqlConnectionString!));
builder.Services.AddTransient<IObjectRepository, SqlObject2DRepository>(o =>
    new SqlObject2DRepository(sqlConnectionString!));

var app = builder.Build();

// Register OpenAPI/Swagger endpoints.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "MySecureBackend API v1");
        options.RoutePrefix = "swagger"; // Access at /swagger
        options.CacheLifetime = TimeSpan.Zero; // Disable caching for development

        // Inject a warning in the Swagger UI if the SQL connection string is missing
        if (!sqlConnectionStringFound)
            options.HeadContent = "<h1 align=\"center\">❌ SqlConnectionString not found ❌</h1>";
    });
    //Was getting bored of the error page default in dev environment


    app.MapGet("/", context =>
    {
        var currentHealthMessage = @$"<!doctype html>
<html>
    <head><title>miniHTML</title></head>
    <body>
        <h1>Dev mode</h1>
        <p>The time on the server is {DateTime.Now:O}</p>
<a href='/swagger/index.html'>Swagger docu</a>
    </body>
</html>";
        context.Response.ContentType = MediaTypeNames.Text.Html;
        context.Response.ContentLength = Encoding.UTF8.GetByteCount(currentHealthMessage);
        return context.Response.WriteAsync(currentHealthMessage);
    });
}
else
{
    // Show the health message directly in non-development environments
    var buildTimeStamp = File.GetCreationTime(Assembly.GetExecutingAssembly().Location);
    var currentHealthMessage =
        $"The API is up 🚀 | Connection string found: {(sqlConnectionStringFound ? "✅" : "❌")} | Build timestamp: {buildTimeStamp}";

    app.MapGet("/", () => currentHealthMessage);
}

// Enforce HTTPS for all requests.
app.UseHttpsRedirection();

// Enable authorization middleware.
app.UseAuthorization();

string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

if (!Directory.Exists(filePath))
{
    Directory.CreateDirectory(filePath);
}

var fileProvider = new PhysicalFileProvider(
    filePath);

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = fileProvider,
    RequestPath = "/uploads" // Access at /files/filename
});

// Register Identity endpoints for account management (register, login, etc.) under /account.
// 👇 uncomment the following line to enable Identity API endpoints to use authentication/authorization
app.MapGroup("/account").MapIdentityApi<IdentityUser>().WithTags("Account");

// Register all controller endpoints for the application.
app.MapControllers().RequireAuthorization();



app.Run();