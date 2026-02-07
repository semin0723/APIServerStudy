using APIServerStudy.Middleware;
using APIServerStudy.Repository;
using ZLogger;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

IConfiguration configuration = builder.Configuration;
builder.Services.Configure<DbConfig>(configuration.GetSection(nameof(DbConfig)));

builder.Services.AddTransient<IGameDB, GameDB>();
builder.Services.AddTransient<IUserAuthDB, UserAuthDB>();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

SettingLogger();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseMiddleware<CheckUserLoginAndLoadData>();

app.UseAuthorization();

app.MapControllers();

app.Run();


void SettingLogger()
{
    ILoggingBuilder logging = builder.Logging;
    logging.ClearProviders();

    string? fileDir = configuration["LogFileDir"];

    if (fileDir == null)
    {
        throw new Exception("LogFileDir is not set in appsettings.json.");
    }

    bool exists = Directory.Exists(fileDir);
    if (!exists)
    {
        Directory.CreateDirectory(fileDir);
    }

    logging.AddZLoggerRollingFile(
        options =>
        {
            options.UseJsonFormatter();
            options.FilePathSelector = (timestamp, sequenceNumber) =>
            $"{fileDir}{timestamp.ToLocalTime():yyyy-MM-dd}_{sequenceNumber:000}.log";
            options.RollingInterval = ZLogger.Providers.RollingInterval.Day;
            options.RollingSizeKB = 4096;
        }
        );
    logging.AddZLoggerConsole(options =>
        { 
            options.UseJsonFormatter(); 
        });
}