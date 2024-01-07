using Hangfire;
using Hangfire.MemoryStorage;
using WebApplicationHangfire.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGenConfig();

builder.Services.AddHangfire(op =>
{
    op.UseMemoryStorage();
});

builder.Services.AddHangfireServer();

var app = builder.Build();

app.UseHangfireDashboard("/jobs");

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerConfig();
}

app.MapHangfireEndpoints();

app.Run();
