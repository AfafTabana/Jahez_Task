

using Hangfire;
using JahezTask.Application;
using JahezTask.Application.Interfaces.Services;
using JahezTask.Infrastructure;
using JahezTask.Persistence;
using JahezTask.Persistence.Data;
using System.Threading;


var builder = WebApplication.CreateBuilder(args);

// Add layers
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddPersistence(builder.Configuration);


builder.Services.AddControllers();

builder.Services.AddOpenApi();

var app = builder.Build();

// Seed the database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var seeder = services.GetRequiredService<DatabaseSeeder>();
        await seeder.SeedAsync();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
   
    app.MapOpenApi(); 

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "JahezTask API v1");
        options.RoutePrefix = "swagger"; 
    });
}


// Enable Hangfire Dashboard for monitoring jobs
 app.UseHangfireDashboard("/hangfire");

// Schedule recurring background job
RecurringJob.AddOrUpdate<INotificationReminderService>(
    recurringJobId: "daily-delayed-books-check",
    methodCall: service => service.CheckDelayedBooksForHangfireAsync(default),
    cronExpression: Cron.Daily,
    timeZone: TimeZoneInfo.Local
);


app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();