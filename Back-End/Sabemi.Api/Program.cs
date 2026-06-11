using Hangfire;
using Sabemi.Api.Configurations;
using Sabemi.Api.Extensions;
using Sabemi.Application;
using Sabemi.Infra;
using Sabemi.Infra.Hubs;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<ILogger, Logger<Program>>();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();
builder.Logging.AddConsole();
builder.Services.AddCustomCors();
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.AddScalar();
    app.UseCors(CorsRegistration.LocalPolicy);
    app.UseHangfireDashboard("/hangfire");
    app.MapGet("/", () => Results.Redirect("/docs")).ExcludeFromDescription();
}

app.UseHttpsRedirection();
app.UseRouting();
app.MapControllers();
app.MapHub<NotificationHub>("/hubs/notifications").AllowAnonymous();
app.Run();