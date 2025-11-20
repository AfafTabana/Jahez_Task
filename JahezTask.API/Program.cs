// Program.cs in JahezTask.API (No namespace, no class, no Main method)

using Hangfire;
using JahezTask.Application;
using JahezTask.Application.Interfaces.Services;
using JahezTask.Persistence;
using JahezTask.Infrastructure;
// Note: You might need to check if 'JahezTask.Persistence' needs to be referenced 
// if its dependencies are registered within 'AddInfrastructure'.

var builder = WebApplication.CreateBuilder(args);

// Add layers
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddPersistence(builder.Configuration);


builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Use the new .NET 8/9 OpenAPI approach
    app.MapOpenApi(); // This creates the /openapi/v1.json endpoint

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "JahezTask API v1");
        options.RoutePrefix = "swagger"; // Optional: Access at /swagger instead of root
    });
}


    // Enable Hangfire Dashboard for monitoring jobs
    app.UseHangfireDashboard("/hangfire");

// Schedule recurring background job
RecurringJob.AddOrUpdate<INotificationReminderService>(
    recurringJobId: "daily-delayed-books-check",
    methodCall: service => service.CheckDelayedBooks(),
    cronExpression: Cron.Daily,
    timeZone: TimeZoneInfo.Local
);


app.UseHttpsRedirection();
app.UseAuthentication();

app.UseAuthorization();


app.UseRouting();

app.UseAuthorization();


app.MapControllers();

app.Run();