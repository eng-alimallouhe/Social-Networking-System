using Microsoft.EntityFrameworkCore;
using SNS.API.DI;
using SNS.Application.DI;
using SNS.Infrastructure.DI;
using SNS.Infrastructure.Services.Loggings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddHttpContextAccessor();

builder.Services.AddSettings(builder.Configuration);

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddLoggingServices();

builder.Services.AddApplication();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
