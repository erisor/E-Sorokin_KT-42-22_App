using Microsoft.EntityFrameworkCore;
using NLog;
using NLog.Web;
using E_Sorokin_KT_42_22_App.Database;
using E_Sorokin_KT_42_22_App.Database.Models;
using E_Sorokin_KT_42_22_App.Database.Configurations;
using E_Sorokin_KT_42_22_App.Services;
using E_Sorokin_KT_42_22_App.Services.ServiceExtensions;
using E_Sorokin_KT_42_22_App.Middlewares;

var builder = WebApplication.CreateBuilder(args);
var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

try
{
    builder.Logging.ClearProviders();
    builder.Host.UseNLog();
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddDbContext<SorokinDBContext>(opts =>
        opts.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection0")));
    builder.Services.AddServices();

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseMiddleware<ExceptionHandlerMiddleware>();

    app.UseAuthorization();

    app.MapControllers();
    app.Run();
}
catch (Exception ex)
{
    logger.Error(ex, "Stopped program because of exeption");
}
finally
{
    LogManager.Shutdown();
}