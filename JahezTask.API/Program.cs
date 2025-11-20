

using Hangfire;
using JahezTask.Application;
using JahezTask.Application.Interfaces.Services;
using JahezTask.Persistence;
using JahezTask.Infrastructure;


var builder = WebApplication.CreateBuilder(args);

// Add layers
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddPersistence(builder.Configuration);


builder.Services.AddControllers();

builder.Services.AddOpenApi();

var app = builder.Build();

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