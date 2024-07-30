using Microsoft.OpenApi.Models;
using Serilog;
using WebApi.Extensions;
using WebApi.Helpers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
string ApiCorsPolicy = "ApiCorsPolicy";
ConfigurationManager configuration = builder.Configuration;

var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
Console.WriteLine(environment + " environment name");

builder.Services.Configure<AppSettings>(configuration.GetSection("AppSettings"));
// Serilog logger config
Serilog.Debugging.SelfLog.Enable(ex => Log.Warning(ex));
builder.Host.UseSerilog((hostingContext, loggerConfiguration) =>
{
    var cfg = loggerConfiguration
        .Enrich.FromLogContext()
        .Enrich.WithProperty("ApplicationName", hostingContext.HostingEnvironment.ApplicationName)
        .MinimumLevel.Information()
        .WriteTo.Console(
            outputTemplate:
            "{NewLine}[{Timestamp:HH:mm:ss} {Level:u3}] :: {Message:lj} {Properties:j}{NewLine}{Exception}")
        .WriteTo.Seq("http://localhost:5341");
    Boolean.TryParse(configuration["DOTNET_RUNNING_IN_CONTAINER"], out bool inContainer);
    if (!inContainer)
    {
        string? logPath = configuration["LogPath"];
        cfg.WriteTo.File(
            logPath,
            outputTemplate:
            "{NewLine}{Timestamp:HH:mm:ss} [{Level}] :: {Message}{NewLine}{Exception}",
            retainedFileCountLimit: 100,
            fileSizeLimitBytes: 104857600,
            rollOnFileSizeLimit: true,
            rollingInterval: RollingInterval.Day

        );
    }
});

builder.Services.AddCoreServices();
//builder.Services.AddSwagger();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Auth api",
        Version = "v1",
        Description = "ASP.NET Core - Role Based Authorization API"
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
try
{
}
catch (Exception ex)
{
    Console.WriteLine(ex);
    Console.WriteLine(ex.ToString());
    Log.Fatal(ex, $"Unhandled exception: {ex.Message}");
}
finally
{
    Console.WriteLine("Shut down completed");
    Log.Information("Shut down completed");
    Log.CloseAndFlush();
}